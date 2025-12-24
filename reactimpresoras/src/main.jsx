import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.jsx'

// 1. Importar CSS de Bootstrap
import 'bootstrap/dist/css/bootstrap.min.css'
// 2. Importar JS de Bootstrap (para tooltips, modales, etc.)
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
// 3. Importar Iconos de Bootstrap (opcional, si los usas)
import 'bootstrap-icons/font/bootstrap-icons.css'

ReactDOM.createRoot(document.getElementById('root')).render(
    <React.StrictMode>
        <App />
    </React.StrictMode>,
)