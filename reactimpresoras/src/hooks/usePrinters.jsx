import { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const API_URL = import.meta.env.VITE_API_URL;
const HUB_URL = `${API_URL.replace('/api', '')}/printerHub`; 

export const usePrinters = () => {
    const [printers, setPrinters] = useState([]);
    const [locations, setLocations] = useState([]); // Esto cargará rápido ahora
    const [connectionId, setConnectionId] = useState(null);
    const [isScanning, setIsScanning] = useState(false);
    const [lastUpdated, setLastUpdated] = useState(null);
    const [error, setError] = useState(null);

    const connectionRef = useRef(null);

    // --- CARGA DE DATOS (SEPARADA) ---
    const loadInitialData = () => {
        setError(null);
        
        // 1. Cargar Ubicaciones (Rápido - Para el desplegable)
        fetch(`${API_URL}/Locations`)
            .then(res => {
                if (!res.ok) throw new Error("Error locations");
                return res.json();
            })
            .then(data => setLocations(data))
            .catch(e => console.error("Error cargando ubicaciones:", e));

        // 2. Cargar Impresoras (Lento - Para la tabla)
        // No bloquea a las ubicaciones
        setIsScanning(true);
        fetch(`${API_URL}/Printers`)
            .then(res => {
                if (!res.ok) throw new Error("Error printers");
                return res.json();
            })
            .then(data => setPrinters(data))
            .catch(e => {
                console.error("Error cargando impresoras:", e);
                setError("No se pudo conectar con el servidor.");
            })
            .finally(() => setIsScanning(false));
    };

    // --- SIGNALR ---
    const triggerScan = async (connId, locId) => {
        try {
            await fetch(`${API_URL}/PrintersRealtime/scan`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ 
                    connectionId: connId, 
                    locationId: parseInt(locId) || 0 // Enviamos el filtro seleccionado
                })
            });
        } catch (err) { console.error(err); }
    };

    useEffect(() => {
        loadInitialData();

        const newConnection = new HubConnectionBuilder()
            .withUrl(HUB_URL)
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        connectionRef.current = newConnection;

        newConnection.on('printerdatareceived', (updatedPrinter) => {
            setPrinters(prev => {
                const index = prev.findIndex(p => p.id === updatedPrinter.id);
                if (index !== -1) {
                    const oldPrinter = prev[index];
                    const newArr = [...prev];
                    
                    const safeLocationId = (updatedPrinter.locationId && updatedPrinter.locationId !== 0) ? updatedPrinter.locationId : oldPrinter.locationId;
                    const safeIp = updatedPrinter.ipAddress || oldPrinter.ipAddress;
                    const safeModel = updatedPrinter.model || oldPrinter.model;
                    const safeLocationName = updatedPrinter.location || oldPrinter.location;

                    newArr[index] = { 
                        ...oldPrinter, ...updatedPrinter,
                        locationId: safeLocationId, ipAddress: safeIp, model: safeModel, location: safeLocationName
                    };
                    return newArr;
                }
                return [...prev, updatedPrinter];
            });
        });

        newConnection.on('scanstarted', () => setIsScanning(true));
        newConnection.on('scancompleted', () => {
            setIsScanning(false);
            setLastUpdated(new Date());
        });

        const startConnection = async () => {
            try {
                await newConnection.start();
                setConnectionId(newConnection.connectionId);
                // OJO: Aquí no forzamos scan automático para no pisar la carga inicial
                // O puedes dejarlo si quieres que escanee "Todo" al arrancar
            } catch (err) { console.error("SignalR Error:", err); }
        };

        startConnection();

        return () => {
            newConnection.off('printerdatareceived');
            newConnection.off('scanstarted');
            newConnection.off('scancompleted');
            newConnection.stop();
        };
    }, []);

    const refreshSinglePrinter = async (id) => {
        if (!connectionId) return;
        try {
            await fetch(`${API_URL}/PrintersRealtime/refresh/${id}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ connectionId })
            });
        } catch (e) { console.error(e); }
    };

    return {
        printers, locations, connectionId, isScanning, lastUpdated, error,
        startScan: (locId) => connectionId && triggerScan(connectionId, locId), // Acepta parámetro locId
        refreshSinglePrinter,
        retryLoad: loadInitialData
    };
};