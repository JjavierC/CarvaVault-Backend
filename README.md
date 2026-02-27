#  CarvaVault – Backend API

Motor central del ecosistema CarvaVault, un sistema SaaS diseñado para automatizar la auditoría de ventas en comercios que dependen de transferencias bancarias.

Este backend centraliza la validación, procesamiento y persistencia de transacciones detectadas en tiempo real mediante un puente de sincronización móvil.

---

##  Descripción General

CarvaVault elimina la verificación manual de pagos al consolidar cada transacción en un panel administrativo profesional, reduciendo errores humanos y mitigando riesgos de fraude.

Este repositorio contiene la API principal encargada de:

- Validación de autenticidad de transacciones
- Persistencia segura de datos
- Gestión multitenant
- Lógica de negocio para auditoría de ventas

---

##  Arquitectura del Ecosistema

CarvaVault opera en tres niveles sincronizados:

###  1. Mobile Synchronization Bridge (Android)
Aplicación en Kotlin que actúa como Edge Device.
- Captura notificaciones de transferencias entrantes
- Estructura los datos
- Envía eventos al backend con latencia mínima

###  2. Backend (Este repositorio)
- ASP.NET 8
- PostgreSQL
- API REST
- Validación y procesamiento de eventos
- Persistencia de datos en la nube

###  3. Web Dashboard
- React
- Tailwind CSS
- Visualización de ventas y flujo de caja
- Gestión de usuarios y control administrativo

---

##  Stack Tecnológico

- ASP.NET Core 8
- PostgreSQL
- Entity Framework Core
- Arquitectura RESTful
- Control de acceso y gestión de usuarios
- Deploy en entorno cloud (configurable)

---

##  Funcionalidades del Backend

- Recepción y validación de eventos de transferencia
- Normalización y estructuración de datos financieros
- Registro persistente de transacciones
- Gestión multitenant para múltiples comercios
- Exposición de endpoints seguros para consumo del dashboard
- Manejo estructurado de errores y logging

---


##  Base de Datos

Sistema relacional basado en PostgreSQL con:

- Integridad referencial
- Normalización de entidades
- Optimización de consultas para reportes financieros
- Índices para consultas de alto volumen

---

##  Objetivo del Proyecto

Automatizar la verificación de pagos bancarios en comercios físicos y digitales, reduciendo la dependencia de confirmaciones manuales y proporcionando trazabilidad completa de cada transacción.

---

## Aprendizajes Técnicos

- Diseño de arquitectura distribuida con Edge Device
- Procesamiento de eventos en tiempo real
- Modelado de datos financieros en PostgreSQL
- Implementación de API REST robusta con ASP.NET
- Sincronización entre aplicación móvil y backend cloud

---

##  Estado del Proyecto

Prototipo funcional con arquitectura SaaS modular y enfoque en escalabilidad.
