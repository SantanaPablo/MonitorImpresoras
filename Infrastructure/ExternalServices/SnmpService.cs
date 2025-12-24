using System.Net;
using System.Net.NetworkInformation;
using Application.Interfaces;
using Dominio.Entities;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Infrastructure.ExternalServices
{
    public class SnmpService : ISnmpService
    {
        private const string Community = "public";
        private const int Timeout = 1000;
        private const int MaxRetries = 3;

        public async Task<Dictionary<string, string>> GetSnmpValuesAsync(string ipAddress, List<string> oids)
        {
            return await Task.Run(() => GetSnmpValuesSync(ipAddress, oids));
        }

        public async Task<bool> PingPrinterAsync(string ipAddress)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var ping = new Ping();
                    var reply = ping.Send(ipAddress, 1000);
                    return reply.Status == IPStatus.Success;
                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task EnrichPrinterDataAsync(Printer printer, OidConfiguration oidConfig)
        {
            var oidList = BuildOidList(oidConfig);
            var responses = await GetSnmpValuesAsync(printer.IpAddress, oidList);

            if (responses.ContainsKey("Error"))
                return;

            // Datos básicos
            printer.MacAddress = responses.GetValueOrDefault(oidConfig.OidMac, "N/A");
            printer.SerialNumber = responses.GetValueOrDefault(oidConfig.OidSerial, "N/A");
            printer.PageCount = ParseInt(responses, oidConfig.OidPageCount);

            // Toner negro (siempre presente)
            printer.TonerLevels.Black = CalculateTonerPercentage(
                responses, oidConfig.OidBlackToner, oidConfig.OidBlackTonerFull);

            // Toners de color (opcionales)
            if (oidConfig.HasColorToner)
            {
                printer.TonerLevels.Cyan = CalculateTonerPercentage(
                    responses, oidConfig.OidCyanToner, oidConfig.OidCyanTonerFull);
                printer.TonerLevels.Magenta = CalculateTonerPercentage(
                    responses, oidConfig.OidMagentaToner, oidConfig.OidMagentaTonerFull);
                printer.TonerLevels.Yellow = CalculateTonerPercentage(
                    responses, oidConfig.OidYellowToner, oidConfig.OidYellowTonerFull);
            }

            // Waste container
            if (!string.IsNullOrWhiteSpace(oidConfig.OidWasteContainer))
                printer.WasteContainerLevel = ParseInt(responses, oidConfig.OidWasteContainer);
            else
                printer.WasteContainerLevel = -1;

            // Image unit
            if (!string.IsNullOrWhiteSpace(oidConfig.OidUnitImage) &&
                !string.IsNullOrWhiteSpace(oidConfig.OidUnitImageFull))
            {
                printer.ImageUnitLevel = CalculateTonerPercentage(
                    responses, oidConfig.OidUnitImage, oidConfig.OidUnitImageFull);
            }
            else
                printer.ImageUnitLevel = -1;
        }

        private Dictionary<string, string> GetSnmpValuesSync(string ipAddress, List<string> oids)
        {
            var results = new Dictionary<string, string>();
            int retryCount = 0;

            while (retryCount < MaxRetries)
            {
                try
                {
                    var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), 161);
                    var communityParam = new OctetString(Community);
                    var requestPdu = oids.Select(oid => new Variable(new ObjectIdentifier(oid))).ToList();

                    var response = Messenger.Get(VersionCode.V2, endpoint, communityParam, requestPdu, Timeout);

                    foreach (var variable in response)
                    {
                        results[variable.Id.ToString()] = variable.Data.ToString();
                    }

                    if (results.Count > 0)
                        return results;
                }
                catch (Exception ex)
                {
                    results["Error"] = ex.Message;
                }

                retryCount++;
                if (retryCount < MaxRetries)
                    Thread.Sleep(500);
            }

            return results;
        }

        private List<string> BuildOidList(OidConfiguration oidConfig)
        {
            var oidList = new List<string>
            {
                oidConfig.OidMac,
                oidConfig.OidModel,
                oidConfig.OidSerial,
                oidConfig.OidPageCount,
                oidConfig.OidBlackToner,
                oidConfig.OidBlackTonerFull
            };

            if (oidConfig.HasColorToner)
            {
                oidList.Add(oidConfig.OidCyanToner!);
                oidList.Add(oidConfig.OidCyanTonerFull!);
                oidList.Add(oidConfig.OidMagentaToner!);
                oidList.Add(oidConfig.OidMagentaTonerFull!);
                oidList.Add(oidConfig.OidYellowToner!);
                oidList.Add(oidConfig.OidYellowTonerFull!);
            }

            if (!string.IsNullOrWhiteSpace(oidConfig.OidWasteContainer))
                oidList.Add(oidConfig.OidWasteContainer);

            if (!string.IsNullOrWhiteSpace(oidConfig.OidUnitImage))
            {
                oidList.Add(oidConfig.OidUnitImage);
                oidList.Add(oidConfig.OidUnitImageFull!);
            }

            return oidList;
        }

        private int CalculateTonerPercentage(Dictionary<string, string> responses,
            string? oidToner, string? oidTonerFull)
        {
            if (string.IsNullOrWhiteSpace(oidToner) || string.IsNullOrWhiteSpace(oidTonerFull))
                return -1;

            int toner = ParseInt(responses, oidToner);
            int tonerFull = ParseInt(responses, oidTonerFull);

            return tonerFull > 0 ? (toner * 100) / tonerFull : 0;
        }

        private int ParseInt(Dictionary<string, string> responses, string oid)
        {
            if (responses.TryGetValue(oid, out var value) && int.TryParse(value.Trim(), out int result))
                return result;
            return 0;
        }
    }
}