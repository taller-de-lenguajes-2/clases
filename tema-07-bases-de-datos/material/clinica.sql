-- ============================================================
-- clinica.sql — base de ejemplo de los temas 07 y 08
-- Pacientes, médicos y los turnos que los relacionan.
-- Se puede ejecutar entero en DB Browser for SQLite
-- (pestaña "Ejecutar SQL") o con:  sqlite3 clinica.db < clinica.sql
-- La carpeta trae clinica.db ya armada con este mismo script.
-- ============================================================

PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS Turno;
DROP TABLE IF EXISTS Paciente;
DROP TABLE IF EXISTS Medico;

CREATE TABLE Paciente (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre   TEXT NOT NULL,
    DNI      TEXT UNIQUE,
    Telefono TEXT
);

CREATE TABLE Medico (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre       TEXT NOT NULL,
    Especialidad TEXT
);

CREATE TABLE Turno (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    PacienteId    INTEGER NOT NULL,
    MedicoId      INTEGER NOT NULL,
    Fecha         TEXT NOT NULL,          -- 'AAAA-MM-DD HH:MM'
    Observaciones TEXT,
    FOREIGN KEY (PacienteId) REFERENCES Paciente(Id),
    FOREIGN KEY (MedicoId)   REFERENCES Medico(Id)
);

INSERT INTO Paciente (Nombre, DNI, Telefono) VALUES
    ('Ana Gómez',     '30111222', '381-4567890'),
    ('Bruno Díaz',    '28999000', NULL),
    ('Carla Ruiz',    '35444555', '381-5550001'),
    ('Mariana Paz',   '40123123', '381-6001234'),
    ('Diego Herrera', '33777888', NULL);

INSERT INTO Medico (Nombre, Especialidad) VALUES
    ('Laura Sosa',   'Clínica médica'),
    ('Martín Vera',  'Pediatría'),
    ('Inés Molina',  'Cardiología');

INSERT INTO Turno (PacienteId, MedicoId, Fecha, Observaciones) VALUES
    (1, 1, '2026-10-20 09:00', 'Control anual'),
    (2, 3, '2026-10-20 10:30', NULL),
    (1, 3, '2026-10-22 11:00', 'Traer electrocardiograma'),
    (3, 2, '2026-10-23 08:15', NULL),
    (4, 1, '2026-10-27 16:00', 'Primera consulta'),
    (5, 1, '2026-11-03 09:30', NULL),
    (3, 3, '2026-11-04 10:00', 'Control de presión'),
    (2, 2, '2026-11-05 17:45', 'Vacunas'),
    (1, 1, '2026-11-10 09:00', 'Resultados de laboratorio'),
    (4, 3, '2026-11-11 12:15', NULL),
    (5, 2, '2026-11-12 08:30', NULL),
    (3, 1, '2026-11-13 15:00', 'Receta');
