-- crear base de datos
CREATE DATABASE TheBear
ON PRIMARY (
NAME='thebear',
FILENAME='C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\thebear_pf.mdf',
SIZE=20MB, 
FILEGROWTH=20MB
)
LOG ON (
NAME='thebear_log',
FILENAME='C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\thebear_log.ldf',
SIZE=40MB, 
FILEGROWTH=10MB
);

-- utilizar base de datos
USE TheBear;

-- crear tablas
CREATE TABLE platillo (
id_platillo INT IDENTITY(1,1) PRIMARY KEY,
nombre VARCHAR(30),
tipo VARCHAR(10) CHECK (tipo IN ('appetizer', 'entree', 'dessert', 'beverage')),
descripcion VARCHAR(100),
gramos SMALLINT,
calorias SMALLINT,
precio MONEY,
fecha_ingreso DATETIME,
fecha_salida DATETIME
);

CREATE TABLE mesa (
id_mesa INT IDENTITY(1,1) PRIMARY KEY,
capacidad TINYINT,
ubicacion VARCHAR(20)
);

CREATE TABLE chef (
id_chef INT IDENTITY(1,1) PRIMARY KEY,
nombre VARCHAR(20),
apellido VARCHAR(25),
puesto VARCHAR(10) CHECK (puesto IN ('owner', 'manager', 'cook', 'server', 'janitor', 'dishwasher', 'host', 'bartender')),
fecha_ingreso DATETIME,
fecha_salida DATETIME,
sueldo MONEY,
);

CREATE TABLE pedido (
id_pedido INT IDENTITY(1,1) PRIMARY KEY,
id_mesa INT REFERENCES mesa (id_mesa),
id_mesero INT REFERENCES chef (id_chef),
fecha DATETIME,
subtotal MONEY,
pagado BIT
);

CREATE TABLE pedido_platillo (
id_pedido INT REFERENCES pedido (id_pedido),
id_platillo INT REFERENCES platillo (id_platillo),
cantidad_pedido TINYINT,
especificaciones VARCHAR(100),
estado VARCHAR(10) CHECK (estado IN ('pending', 'preparing', 'ready', 'served', 'canceled')),
CONSTRAINT pk_pedido_menu PRIMARY KEY (id_pedido, id_platillo)
);

-- añadir datos a tablas
INSERT INTO platillo (nombre, tipo, descripcion, gramos, calorias, precio, fecha_ingreso, fecha_salida)
VALUES 
('Beef Tartare', 'appetizer', 'Raw beef, quail egg, capers', 150, 250, 18.00, '2023-11-01', NULL),
('Porchetta', 'entree', 'Slow-roasted pork with herbs', 300, 600, 35.00, '2023-11-01', NULL),
('Pavlova', 'dessert', 'Meringue with cream and berries', 100, 200, 12.00, '2023-11-01', NULL),
('Negroni', 'beverage', 'Gin, Campari, sweet vermouth', 150, 200, 14.00, '2023-11-01', NULL),
('Ricotta Gnocchi', 'entree', 'Homemade gnocchi with ricotta', 250, 500, 22.00, '2023-11-01', NULL),
('Affogato', 'dessert', 'Espresso over vanilla gelato', 80, 150, 8.00, '2023-11-01', NULL),
('Caesar Salad', 'appetizer', 'Romaine, croutons, parmesan', 120, 150, 10.00, '2023-11-01', NULL),
('Espresso Martini', 'beverage', 'Vodka, espresso, coffee liqueur', 120, 180, 13.00, '2023-11-01', NULL);

INSERT INTO mesa (capacidad, ubicacion)
VALUES 
(2, 'Ventana'),
(4, 'Centro'),
(6, 'Terraza'),
(8, 'Privada'),
(2, 'Esquina'),
(4, 'Cerca de la barra'),
(6, 'Al fondo');

INSERT INTO chef (nombre, apellido, puesto, fecha_ingreso, fecha_salida, sueldo)
VALUES 
('Carmen', 'Berzatto', 'owner', '2023-01-01', NULL, 75000),
('Sydney', 'Adamu', 'manager', '2023-01-15', NULL, 60000),
('Richard', 'Jeremiah', 'cook', '2023-02-01', NULL, 45000),
('Marcus', 'Harris', 'cook', '2023-03-01', NULL, 42000),
('Tina', 'Morales', 'server', '2023-04-01', NULL, 35000),
('Ebraheim', 'Elhadi', 'dishwasher', '2023-02-10', NULL, 30000),
('Natalie', 'Berzatto', 'host', '2023-05-01', NULL, 32000),
('Gary', 'Sotelo', 'janitor', '2023-06-01', NULL, 28000);

INSERT INTO pedido (id_mesa, id_mesero, fecha, subtotal, pagado)
VALUES 
(1, 5, '2023-11-01 18:01:43', 50.00, 1),
(2, 5, '2023-11-01 18:12:13', 75.00, 1),
(3, 5, '2023-11-01 18:19:45', 120.00, 1),
(4, 5, '2023-11-01 18:25:32', 200.00, 0),
(5, 5, '2023-11-01 18:26:22', 45.00, 1),
(6, 5, '2023-11-01 18:31:54', 85.00, 0),
(2, 5, '2023-11-01 18:40:32', 95.00, 1);

INSERT INTO pedido_platillo (id_pedido, id_platillo, cantidad_pedido, especificaciones, estado)
VALUES 
(1, 1, 1, 'Sin alcaparras', 'served'),
(1, 2, 2, '', 'served'),
(2, 3, 1, 'Extra crema', 'preparing'),
(3, 4, 1, '', 'served'),
(4, 5, 2, 'Sin queso', 'ready'),
(5, 6, 1, '', 'served'),
(6, 7, 3, 'Extra aderezo', 'pending');

-- 3 indices
-- indice en la columna `tipo` de la tabla platillo para acelerar consultas por tipo de platillo
CREATE INDEX idx_tipo_platillo ON platillo(tipo);

-- indice en la columna `ubicacion` de la tabla mesa para agilizar búsquedas por ubicación de la mesa
CREATE INDEX idx_ubicacion_mesa ON mesa(ubicacion);

-- indice en la columna `estado` de la tabla pedido_platillo para optimizar consultas de estado de platillos en pedidos
CREATE INDEX idx_estado_pedido_platillo ON pedido_platillo(estado);

-- 1 trigger
-- trigger para actualizar el estado del pedido cuando todos los platillos asociados estén servidos
CREATE TRIGGER actualizar_estado_pedido ON pedido_platillo
AFTER UPDATE
AS
BEGIN
    DECLARE @id_pedido INT;
    SELECT @id_pedido = id_pedido FROM inserted;

    IF NOT EXISTS (SELECT 1 FROM pedido_platillo WHERE id_pedido = @id_pedido AND estado <> 'served')
    BEGIN
        UPDATE pedido SET pagado = 1 WHERE id_pedido = @id_pedido;
    END
END;

-- 2 funciones
-- funcion para calcular el total de calorías en un pedido
CREATE FUNCTION calcular_calorias_pedido (@id_pedido INT)
RETURNS INT
AS
BEGIN
    DECLARE @total_calorias INT;
    SELECT @total_calorias = SUM(p.cantidad_pedido * pl.calorias)
    FROM pedido_platillo p
    JOIN platillo pl ON p.id_platillo = pl.id_platillo
    WHERE p.id_pedido = @id_pedido;
    RETURN @total_calorias;
END;

-- funcion para calcular el subtotal de un pedido (sin incluir descuentos o impuestos)
CREATE FUNCTION calcular_subtotal_pedido (@id_pedido INT)
RETURNS MONEY
AS
BEGIN
    DECLARE @subtotal MONEY;
    SELECT @subtotal = SUM(p.cantidad_pedido * pl.precio)
    FROM pedido_platillo p
    JOIN platillo pl ON p.id_platillo = pl.id_platillo
    WHERE p.id_pedido = @id_pedido;
    RETURN @subtotal;
END;

-- 1 vista
-- vista para generar un reporte de ventas por mesero y mesa
CREATE VIEW ventas_mesero_mesa AS
SELECT 
    m.nombre AS Mesero,
    e.ubicacion AS UbicacionMesa,
    p.subtotal,
    p.pagado,
    COUNT(pl.id_platillo) AS TotalPlatillos
FROM pedido p
JOIN chef m ON p.id_mesero = m.id_chef
JOIN mesa e ON p.id_mesa = e.id_mesa
JOIN pedido_platillo pl ON p.id_pedido = pl.id_pedido
GROUP BY m.nombre, e.ubicacion, p.subtotal, p.pagado;

-- stored procedure
-- Procedimiento almacenado para obtener un reporte de ventas diarias
CREATE PROCEDURE reporte_ventas_diarias (@fecha DATE)
AS
BEGIN
    SELECT 
        p.id_pedido,
        m.nombre AS Mesero,
        SUM(pl.cantidad_pedido * pr.precio) AS TotalVentas
    FROM pedido p
    JOIN chef m ON p.id_mesero = m.id_chef
    JOIN pedido_platillo pl ON p.id_pedido = pl.id_pedido
    JOIN platillo pr ON pl.id_platillo = pr.id_platillo
    WHERE CONVERT(DATE, p.fecha) = @fecha
    GROUP BY p.id_pedido, m.nombre;
END;

