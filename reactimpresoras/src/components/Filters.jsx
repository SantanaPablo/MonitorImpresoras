import React, { useState } from 'react';

const Filters = ({ filters, setFilters, locations, onRefresh }) => {
    
    // Estado local para bloquear el botón general
    const [isCoolingDown, setIsCoolingDown] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFilters(prev => ({ ...prev, [name]: value }));
    };

    const handleRefreshClick = () => {
        if (isCoolingDown) return; // Evitar doble clic

        setIsCoolingDown(true);
        onRefresh(); // Ejecutar acción

        // Bloqueo de 3 segundos para el barrido general
        setTimeout(() => setIsCoolingDown(false), 3000);
    };

    return (
        <div className="row g-2 mb-3 align-items-end bg-light p-2 rounded border">
            {/* 1. Ubicación */}
            <div className="col-6 col-md-4 col-lg-2">
                <label className="form-label small fw-bold mb-1">Ubicación</label>
                <select className="form-select form-select-sm" name="locationId" value={filters.locationId} onChange={handleChange}>
                    <option value="0">Todas</option>
                    {locations && locations.map((loc, i) => {
                        const id = loc.id || loc.Id;
                        const name = loc.nombre || loc.Nombre || loc.name;
                        return id ? <option key={`${id}-${i}`} value={id}>{name}</option> : null;
                    })}
                </select>
            </div>

            {/* 2. Modelo */}
            <div className="col-6 col-md-4 col-lg-2">
                <label className="form-label small fw-bold mb-1">Modelo</label>
                <input type="text" className="form-control form-control-sm" name="model" value={filters.model} onChange={handleChange}/>
            </div>

            {/* 3. Sector */}
            <div className="col-6 col-md-4 col-lg-2">
                <label className="form-label small fw-bold mb-1">Sector</label>
                <input type="text" className="form-control form-control-sm" name="sector" value={filters.sector} onChange={handleChange}/>
            </div>

            {/* 4. IP */}
            <div className="col-6 col-md-4 col-lg-2">
                <label className="form-label small fw-bold mb-1">IP</label>
                <input type="text" className="form-control form-control-sm" name="ip" value={filters.ip} onChange={handleChange}/>
            </div>

            {/* 5. Ordenar */}
            <div className="col-6 col-md-4 col-lg-2">
                <label className="form-label small fw-bold mb-1">Ordenar por</label>
                <select className="form-select form-select-sm" name="sortBy" value={filters.sortBy} onChange={handleChange}>
                    <option value="Modelo">Modelo</option>
                    <option value="Ubicacion">Ubicación</option>
                    <option value="IP">IP</option>
                    <option value="Toner">% Tóner</option>
                </select>
            </div>

            {/* BOTÓN ACTUALIZAR (PROTEGIDO) */}
            <div className="col-12 col-md-4 col-lg-2">
                <button 
                    className="btn btn-primary btn-sm w-100 fw-bold" 
                    onClick={handleRefreshClick}
                    disabled={isCoolingDown}
                >
                    {isCoolingDown ? (
                        <>
                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                            Espere...
                        </>
                    ) : (
                        <>
                            <i className="bi bi-funnel me-1"></i> Actualizar
                        </>
                    )}
                </button>
            </div>
        </div>
    );
};

export default Filters;