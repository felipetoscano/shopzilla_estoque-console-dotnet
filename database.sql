CREATE DATABASE DB_ESTOQUE;

USE DB_ESTOQUE;

DROP TABLE ESTOQUE;

CREATE TABLE ESTOQUE(
	ID INT IDENTITY(1, 1) PRIMARY KEY,
	SKU VARCHAR(20) UNIQUE NOT NULL,
	QUANTIDADE INT NOT NULL
);

INSERT INTO ESTOQUE (SKU, QUANTIDADE) VALUES ('CELNVIPHONE12', 100);