import React, { useState, useEffect, useRef } from 'react';

const PrinterRow = ({ printer, onRefreshSingle }) => {
    
    // 1. Estado para el Flash amarillo (Feedback visual de "Dato Nuevo")
    const [flash, setFlash] = useState(false);
    
    // 2. Estado para bloquear el botón (Anti-spam)
    const [isCoolingDown, setIsCoolingDown] = useState(false);
    
    const isMounted = useRef(false);

    // Efecto: Activa el flash cuando cambia la data de la impresora
    useEffect(() => {
        if (isMounted.current) {
            setFlash(true);
            const timer = setTimeout(() => setFlash(false), 1500);
            return () => clearTimeout(timer);
        } else {
            isMounted.current = true;
        }
    }, [printer]);

    // MANEJADOR DE CLIC SEGURO
    const handleRefreshClick = () => {
        if (isCoolingDown) return; // Si está "caliente", no hace nada

        setIsCoolingDown(true); // Bloqueamos
        onRefreshSingle(printer.id); // Ejecutamos

        // Desbloqueamos a los 5 segundos
        setTimeout(() => {
            setIsCoolingDown(false);
        }, 5000); 
    };

    // LÓGICA DE COLORES DE FILA
    const getRowColor = (p) => {
        // Si no tiene serial, es que falló SNMP -> Gris
        if (!p.serialNumber || p.serialNumber === "N/A") return "table-secondary";

        const black = p.tonerLevels?.black ?? -1;
        const cyan = p.tonerLevels?.cyan ?? -1;
        const magenta = p.tonerLevels?.magenta ?? -1;
        const yellow = p.tonerLevels?.yellow ?? -1;
        const has = (val) => val !== -1 && val !== null;

        // Crítico (Rojo)
        const isCritical = 
            (has(black) && black <= 10) || 
            (has(cyan) && cyan <= 10) || 
            (has(magenta) && magenta <= 10) || 
            (has(yellow) && yellow <= 10);

        if (isCritical) return "table-danger";

        // Advertencia (Amarillo)
        const isWarning = 
            (has(black) && black <= 50) || 
            (has(cyan) && cyan <= 50) || 
            (has(magenta) && magenta <= 50) || 
            (has(yellow) && yellow <= 50);

        if (isWarning) return "table-warning";

        // Todo OK (Verde)
        return "table-success";
    };

    // HELPER: BARRAS DE PROGRESO (Con X roja para nulos)
    const renderProgressBar = (value, colorClass) => {
        if (value === null || value === undefined || value < 0) {
            return <span className="text-danger fw-bold" style={{fontSize: '12px'}}>X</span>;
        }
        
        return (
            <div className="progress progress-thin" style={{ width: '90px', margin: '0 auto' }}>
                <div 
                    className={`progress-bar ${colorClass}`} 
                    role="progressbar" 
                    style={{ width: `${value}%` }}
                    aria-valuenow={value} aria-valuemin="0" aria-valuemax="100"
                >
                    {value}%
                </div>
            </div>
        );
    };

    // HELPER: WASTE CONTAINER
    const renderWasteBadge = (value) => {
        if (value > 0) return <span className="badge rounded-pill text-bg-success badge-mini">OK</span>;
        if (value === 0) return <span className="badge rounded-pill text-bg-danger badge-mini">Reemplazar</span>;
        return <span className="text-danger fw-bold" style={{fontSize: '12px'}}>X</span>;
    };

    const rowClass = getRowColor(printer);
    const finalClass = `${rowClass} ${flash ? 'row-updated' : ''}`;
    const pingClass = printer.status === "Online" ? "text-success" : "text-danger";

    return (
        <tr className={finalClass} style={{ transition: 'background-color 0.2s' }}>
            <td className="fw-bold">{printer.model}</td>
            
            <td>
                <a href={`http://${printer.ipAddress}`} target="_blank" rel="noreferrer">
                    {printer.ipAddress}
                </a>
            </td>
            
            <td>{printer.location}</td>
            <td className="text-left">{printer.macAddress || "N/A"}</td>
            <td className="text-left">{printer.serialNumber}</td>
            
            <td className="text-center fw-bold">{printer.pageCount?.toLocaleString()}</td>
            
            <td className="text-center">{renderProgressBar(printer.tonerLevels?.black, 'bg-dark')}</td>
            <td className="text-center">{renderProgressBar(printer.tonerLevels?.cyan, 'bg-info')}</td>
            <td className="text-center">{renderProgressBar(printer.tonerLevels?.magenta, 'bg-danger')}</td>
            <td className="text-center">{renderProgressBar(printer.tonerLevels?.yellow, 'bg-warning')}</td>
            
            <td className="text-center">{renderWasteBadge(printer.wasteContainerLevel)}</td>
            <td className="text-center">{renderProgressBar(printer.imageUnitLevel, 'bg-success')}</td>
            
            <td className="text-center">
                <i className={`bi bi-circle-fill ${pingClass}`} style={{fontSize: '8px'}}></i>
            </td>
            
            {/* BOTÓN CON COOLDOWN */}
            <td className="text-center">
                <button 
                    className="btn btn-link p-0 text-secondary" 
                    onClick={handleRefreshClick}
                    disabled={isCoolingDown} 
                    title={isCoolingDown ? "Espere..." : "Actualizar ahora"}
                    style={{ lineHeight: 1, cursor: isCoolingDown ? 'not-allowed' : 'pointer' }}
                >
                    <i 
                        className={`bi bi-arrow-clockwise ${isCoolingDown ? 'animate-spin text-muted' : ''}`} 
                        style={{ fontSize: '14px', display: 'inline-block' }}
                    ></i>
                </button>
            </td>
        </tr>
    );
};

export default PrinterRow;