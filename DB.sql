CREATE DATABASE IF NOT EXISTS `InmobiliariaSAD` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `InmobiliariaSAD`;

CREATE TABLE IF NOT EXISTS `propietario` (
    `IdPropietario` INT NOT NULL AUTO_INCREMENT,
    `Nombre` VARCHAR(50) NOT NULL,
    `Apellido` VARCHAR(50) NOT NULL,
    `Dni` VARCHAR(30) NOT NULL,
    `Telefono` VARCHAR(30) NULL,
    `Email` VARCHAR(100) NOT NULL,
    `Clave` VARCHAR(255) NOT NULL,
    PRIMARY KEY (`IdPropietario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `inquilino` (
    `IdInquilino` INT NOT NULL AUTO_INCREMENT,
    `Nombre` VARCHAR(50) NOT NULL,
    `Apellido` VARCHAR(50) NOT NULL,
    `Dni` VARCHAR(30) NOT NULL,
    `Telefono` VARCHAR(30) NULL,
    `Email` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`IdInquilino`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;