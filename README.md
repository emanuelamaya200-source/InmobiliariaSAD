# InmobiliariaSAD
.net practica

> El sistema trata de la informatización de la gestión de alquileres
temporarios de propiedades inmuebles que realiza una agencia
inmobiliaria.

---

## 👥 Integrantes del Grupo

* **Leandro Amaya** - *emanuelamaya200@gmail.com* - [@emanuelamaya200-source]
* **Fabian D'Agata** - *angelfabiandagata@gmail.com* - [@angelfabiandagata-ui]
* **Lucas Serrano** - *serranolucas1000@gmail.com* - [@lucasserrano7]

---

## 📐 Modelado de Datos

A continuación se presenta el esquema del modelo de datos correspondiente a la aplicación:

![Diagrama del Proyecto](./img/DEG.png)

---

## 🛠️ Requisitos Previos

* [.NET SDK](https://dotnet.microsoft.com/download) (versión 8.0 o superior)
* Servidor MySQL / MariaDB (por ejemplo, mediante [XAMPP](https://www.apachefriends.org/) o MySQL Workbench)

---

## Comandos

Configurar la Base de Datos:

Iniciar el servidor MySQL (por ejemplo, desde el panel de control de XAMPP).
o instalando el driver de mySql (corre en segundo plano hasta detenerlo)

Abrir el gestor de base de datos preferido (phpMyAdmin, MySQL Workbench, DBeaver etc.).

Ejecutar el script SQL incluido en el proyecto:

* Archivo: DB.sql
(Ejecutando este archivo ya se crea la base de datos)

Verificar en el archivo `appsettings.json` que el usuario y la contraseña coincidan con los de tu entorno local:

"ConnectionString":  "Server=localhost;Database=InmobiliariaSAD;User=root;Password=;"


Para inicializar: - dotnet run  

