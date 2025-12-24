import { useState, useMemo } from 'react';
import { usePrinters } from '../hooks/usePrinters';
import PrinterRow from '../components/PrinterRow';
import Filters from '../components/Filters';

// Helper para convertir IP "192.168.1.10" en número comparable
const ipToNum = (ip) => {
    if (!ip) return 0;
    return ip.split('.').map(d => parseInt(d)).reduce((acc, n) => acc * 256 + n, 0);
};

const Dashboard = () => {
    const { 
        printers, locations, isScanning, lastUpdated, error,
        startScan, refreshSinglePrinter, retryLoad 
    } = usePrinters();

    const [filters, setFilters] = useState({
        locationId: "0",
        model: "",
        ip: "",
        sector: "", // <--- 1. AGREGADO: Estado para el sector
        sortBy: "Modelo"
    });

    // Usamos useMemo para optimizar el filtrado y ordenamiento
    const processedPrinters = useMemo(() => {
        // 1. FILTRADO
        let result = printers.filter(p => {
            const pLocId = p.locationId || 0; 
            const matchLoc = filters.locationId === "0" || pLocId.toString() === filters.locationId.toString();
            
            const pModel = p.model || "";
            const matchModel = filters.model === "" || pModel.toLowerCase().includes(filters.model.toLowerCase());
            
            const pIp = p.ipAddress || ""; 
            const matchIp = filters.ip === "" || pIp.includes(filters.ip);

            // <--- 2. AGREGADO: Lógica de Sector
            // Busca si lo que escribiste en "Sector" está dentro del nombre de la ubicación
            const pLocName = p.location || ""; 
            const matchSector = filters.sector === "" || pLocName.toLowerCase().includes(filters.sector.toLowerCase());

            return matchLoc && matchModel && matchIp && matchSector;
        });

        // 2. ORDENAMIENTO
        result.sort((a, b) => {
            switch (filters.sortBy) {
                case "Ubicacion":
                    return (a.location || "").localeCompare(b.location || "");
                case "IP":
                    return ipToNum(a.ipAddress) - ipToNum(b.ipAddress);
                case "Toner":
                    // Ordenamos por Toner Negro (Ascendente)
                    return (a.tonerLevels?.black ?? 100) - (b.tonerLevels?.black ?? 100);
                case "Modelo":
                default:
                    return (a.model || "").localeCompare(b.model || "");
            }
        });

        return result;

    }, [printers, filters]);

    const formatTime = (date) => {
        if (!date) return "Pendiente...";
        return new Date(date).toLocaleString('es-AR', {
            day: '2-digit', month: '2-digit', 
            hour: '2-digit', minute: '2-digit', second: '2-digit',
            hour12: false
        });
    };

    // --- VISTA DE ERROR CRÍTICO ---
    if (error) {
        return (
            <div className="d-flex flex-column align-items-center justify-content-center vh-100 text-center">
                <div className="alert alert-danger p-5 shadow" role="alert">
                    <h1 className="display-4"><i className="bi bi-exclamation-triangle"></i></h1>
                    <h4 className="alert-heading">No se pudo conectar al servidor</h4>
                    <p>{error}</p>
                    <hr />
                    <button className="btn btn-danger btn-lg" onClick={retryLoad}>
                        <i className="bi bi-arrow-clockwise me-2"></i> Reintentar Conexión
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="container-fluid py-4 px-4">
            
            <div className="d-flex justify-content-between align-items-center mb-4 bg-white p-3 rounded shadow-sm border">
                
                <h4 className="mb-0 text-secondary fw-bold">
                    <i className="bi bi-printer-fill me-2 text-primary"></i>
                    Monitor de Impresoras
                </h4>

                <div className="d-flex align-items-center gap-3 small">
                    <div className="d-flex align-items-center" title="Estado de conexión con el servidor">
                        {isScanning ? (
                            <i className="bi bi-activity animate-pulse text-primary fs-5 me-2"></i>
                        ) : (
                            <i className="bi bi-check-circle-fill text-success fs-5 me-2"></i>
                        )}
                        <span className="text-muted d-none d-md-inline">
                            {isScanning ? "Escaneando..." : "Servicio activo"}
                        </span>
                    </div>

                    <div className="border-start ps-3 text-muted">
                        <i className="bi bi-clock me-1"></i>
                        <span className="text-muted ms-1">Última Actualización </span>
                        {formatTime(lastUpdated)}
                    </div>

                    <div className="border-start ps-3">
                        <span className="fw-bold text-dark fs-6">
                            {processedPrinters.length}
                        </span>
                        <span className="text-muted ms-1">impresoras</span>
                    </div>
                </div>
            </div>

            <Filters 
                filters={filters} 
                setFilters={setFilters} 
                locations={locations} 
                onRefresh={() => startScan(filters.locationId)} 
            />

            {/* TABLA */}
            <div className="card shadow-sm border-0">
                {printers.length === 0 && isScanning ? (
                    <div className="text-center py-5">
                        <div className="spinner-border text-primary" style={{width: '3rem', height: '3rem'}}></div>
                        <p className="mt-3 text-muted">Cargando datos...</p>
                    </div>
                ) : (
                    <div className="table-responsive">
                        <table 
                            className="table table-bordered table-sm table-hover text-nowrap align-middle mb-0 w-100" 
                            style={{ fontSize: '12px' }}
                        >
                            <thead className="table-dark sticky-top">
                                <tr>
                                    <th>Modelo</th>
                                    <th>IP</th>
                                    <th>Ubicación</th>
                                    <th>Host</th>
                                    <th>Serial</th>
                                    <th className="text-center">Págs</th>
                                    <th className="text-center">Negro</th>
                                    <th className="text-center">Cian</th>
                                    <th className="text-center">Magenta</th>
                                    <th className="text-center">Amarillo</th>
                                    <th className="text-center">Waste</th>
                                    <th className="text-center">Imagen</th>
                                    <th className="text-center">Ping</th>
                                    <th className="text-center">Re-Scan</th>
                                </tr>
                            </thead>
                            <tbody>
                                {processedPrinters.length > 0 ? (
                                    processedPrinters.map(printer => (
                                        <PrinterRow 
                                            key={printer.id} 
                                            printer={printer} 
                                            onRefreshSingle={refreshSinglePrinter}
                                        />
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="14" className="text-center py-4 text-muted">
                                            No hay impresoras que coincidan con los filtros.
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Dashboard;