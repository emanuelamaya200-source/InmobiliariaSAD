CREATE TABLE Propietario (
    id INT PRIMARY KEY,
    nombre VARCHAR(50),
    apellido VARCHAR(50),
    dni VARCHAR(30) NOT NULL,
    telefono VARCHAR(30),
    email VARCHAR not NULL,
)

CREATE TABLE Inquilino (
    id INT PRIMARY KEY,
    nombre VARCHAR(50),
    apellido VARCHAR(50),
    dni VARCHAR(30) NOT NULL,
    telefono VARCHAR(30),
    email VARCHAR not NULL,
)