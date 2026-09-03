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


CREATE TABLE IF NOT EXISTS `tipoInmueble` (
    `IdTipoInmueble` INT NOT NULL AUTO_INCREMENT,
    `Descripcion` VARCHAR(50) NOT NULL,
    PRIMARY KEY (`IdTipoInmueble`)
)ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `inmueble` (
    `IdInmueble` INT NOT NULL AUTO_INCREMENT,
    `Direccion` VARCHAR(150) NOT NULL,
    `Cupo` INT NOT NULL DEFAULT 1,
    `PrecioPorDia` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `PorcentajeReserva` DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    `Latitud` DECIMAL(10,8) NULL,
    `Longitud` DECIMAL(11,8) NULL,
    `PropietarioId` INT NOT NULL,
    `Portada` VARCHAR(255) NULL,
    PRIMARY KEY (`IdInmueble`),
    CONSTRAINT `FK_Inmueble_Propietario` 
        FOREIGN KEY (`PropietarioId`) 
        REFERENCES `propietario` (`IdPropietario`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


INSERT INTO `propietario` (`Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Clave`) VALUES
('Carlos', 'Gómez', '28456123', '2664123456', 'carlos.gomez@gmail.com', '123456'),
('Mariana', 'López', '32987654', '2664987654', 'mariana.lopez@hotmail.com', '123456'),
('Jorge', 'Fernández', '25111222', '2664555888', 'jorge.fernandez@yahoo.com', '123456'),
('Lucía', 'Martínez', '35666777', '2664333222', 'lucia.martinez@gmail.com', '123456'),
('Esteban', 'Pérez', '30444555', '2664777111', 'esteban.perez@outlook.com', '123456'),
('Valeria', 'Romero', '38123987', '2664666444', 'valeria.romero@gmail.com', '123456'),
('Martín', 'Sosa', '27999000', '2664888333', 'martin.sosa@gmail.com', '123456'),
('Camila', 'Torres', '36777888', '2664222999', 'camila.torres@hotmail.com', '123456'),
('Gonzalo', 'Díaz', '33444111', '2664111000', 'gonzalo.diaz@gmail.com', '123456'),
('Florencia', 'Benítez', '39123456', '2664444777', 'florencia.benitez@gmail.com', '123456'),
('Roberto', 'Acosta', '22333444', '2664999111', 'roberto.acosta@yahoo.com', '123456'),
('Paula', 'Medina', '34555666', '2664000333', 'paula.medina@gmail.com', '123456');

INSERT INTO `inquilino` (`Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`) VALUES
('Matías', 'Suárez', '37890123', '2664159753', 'matias.suarez@gmail.com'),
('Agustina', 'Navarro', '40123789', '2664753951', 'agustina.navarro@outlook.com'),
('Federico', 'Morales', '31456789', '2664369258', 'federico.morales@hotmail.com'),
('Julieta', 'Herrera', '38987321', '2664258369', 'julieta.herrera@gmail.com'),
('Nicolás', 'Castro', '36258147', '2664147258', 'nicolas.castro@gmail.com'),
('Sofía', 'Vega', '41234567', '2664951753', 'sofia.vega@yahoo.com'),
('Diego', 'Rios', '29874512', '2664852963', 'diego.rios@gmail.com'),
('Micaela', 'Flores', '35741852', '2664321654', 'micaela.flores@hotmail.com'),
('Ignacio', 'Peralta', '33698521', '2664654987', 'ignacio.peralta@gmail.com'),
('Daniela', 'Molina', '39852147', '2664789123', 'daniela.molina@gmail.com');

INSERT INTO `tipoInmueble` (`Descripcion`) VALUES 
('Casa'),
('Departamento'),
('Local Comercial'),
('Cochera'),
('Terreno');