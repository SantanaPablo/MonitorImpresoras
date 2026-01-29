# 📊 Monitor de Impresoras
> **Sistema de monitoreo de suministros y gestión de activos mediante protocolo SNMP.**

**Monitor de Impresoras** es una solución de backend y frontend diseñada para centralizar el control de impresoras. A diferencia de un monitor convencional, este sistema se comunica directamente con el hardware mediante el protocolo **SNMP (Simple Network Management Protocol)**, permitiendo obtener métricas precisas de consumibles y estado operativo en tiempo real sin depender de software de terceros.

---

## 🎯 Capacidades del Sistema
El núcleo del motor transforma respuestas crudas de OIDs (*Object Identifiers*) en información de negocio procesable:

* **Gestión de Suministros:** Monitoreo dinámico de niveles de tóner con cálculo porcentual basado en capacidad máxima.
* **Auditoría de Activos:** Extracción automática de Números de Serie, direcciones MAC y modelos directamente desde el firmware.
* **Contador de Páginas:** Seguimiento preciso de la vida útil del equipo para mantenimientos preventivos.
* **Diagnóstico de Red:** Verificación de disponibilidad mediante Ping asíncrono y reintentos configurables para redes inestables.
* **Configuración por OIDs:** Flexibilidad total para soportar marcas como HP, Samsung, Lexmark mediante el mapeo dinámico de OIDs en la base de datos.

---

## 🛠️ Stack Tecnológico

| Capa | Tecnología |
| :--- | :--- |
| **Backend** | .NET 8 Web API |
| **Protocolo** | SNMP v2 (Lextm.SharpSnmpLib) |
| **Frontend** | React |
| **Base de Datos** | MySQL (Persistencia de OIDs y registros) |
| **Arquitectura** | Clean Architecture, Repository Pattern |

---

## 🏗️ Arquitectura de Infraestructura
El sistema aplica una separación clara de responsabilidades para asegurar la escalabilidad:

* **Domain:** Entidades de núcleo como `Printer`, `TonerLevels` y `OidConfiguration`.
* **Application:** Casos de uso e interfaces de servicio (`ISnmpService`).
* **Infrastructure:** Implementación del `SnmpService` con lógica de reintentos, conexión a BBDD.
* **API:** Controladores REST para la interacción con el Dashboard.

# Desarrollado por Pablo Santana
