import React from 'react';

const LoadingSpinner = ({ text = "Escaneando red e impresoras..." }) => {
    return (
        <div className="d-flex flex-column justify-content-center align-items-center py-5">
            <div className="spinner-border text-primary" style={{ width: '3rem', height: '3rem' }} role="status">
                <span className="visually-hidden">Cargando...</span>
            </div>
            <p className="mt-3 text-muted fs-5 animate-pulse">{text}</p>
        </div>
    );
};

export default LoadingSpinner;