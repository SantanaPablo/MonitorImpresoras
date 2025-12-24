import React from 'react';
import Dashboard from './pages/Dashboard';

function App() {
    return (
        <div className="d-flex flex-column min-vh-100 bg-light">
            {/* 1. Navbar Superior */}
            <nav className="navbar navbar-expand-lg navbar-dark bg-dark shadow mb-3">
                <div className="container-fluid">
                    <a className="navbar-brand fw-bold" href="/">
                        {/* Icono de Bootstrap (asegúrate de incluir los iconos en index.html o npm) */}
                        <i className="bi bi-printer-fill me-2"></i>
                        Monitor de Impresoras
                    </a>

                    <div className="d-flex align-items-center">
                        <span className="badge bg-secondary me-2">
                            <i className="bi bi-hdd-network me-1"></i>
                            Sistema Online
                        </span>
                    </div>
                </div>
            </nav>

            {/* 2. Contenido Principal */}
            <main className="flex-grow-1">
                <Dashboard />
            </main>

            {/* 3. Footer (Opcional) */}
            <footer className="bg-white text-center py-3 border-top mt-auto">
                <div className="container">
                    <span className="text-muted small">
                        &copy; {new Date().getFullYear()} - Panel de Control de Impresoras
                    </span>
                </div>
            </footer>
        </div>
    );
}

export default App;