-- 1. Crear tabla de Productos
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "BasePrice" DECIMAL(18, 2) NOT NULL
);

-- 2. Crear tabla de Separatas (Campañas)
CREATE TABLE IF NOT EXISTS "Separatas" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "StartDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "EndDate" TIMESTAMP WITH TIME ZONE NOT NULL
);

-- 3. Crear tabla de SeparataItems (Relación muchos a muchos con data extra)
CREATE TABLE IF NOT EXISTS "SeparataItems" (
    "Id" UUID PRIMARY KEY,
    "SeparataId" UUID NOT NULL,
    "ProductId" UUID NOT NULL,
    "PromotionType" VARCHAR(50) NOT NULL,
    "PromotionValue" DECIMAL(18, 2) NOT NULL,
    CONSTRAINT "FK_SeparataItems_Separatas" FOREIGN KEY ("SeparataId") REFERENCES "Separatas" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SeparataItems_Products" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

-- 4. Seed Data: Productos iniciales para la prueba
INSERT INTO "Products" ("Id", "Name", "BasePrice") VALUES
('3fa85f64-5717-4562-b3fc-2c963f66afa6', 'Laptop Pro', 4500000.00),
('11111111-2222-3333-4444-555555555555', 'Mouse Gamer', 150000.00),
('99999999-8888-7777-6666-555555555555', 'Teclado Mecánico', 350000.00),
('77777777-6666-5555-4444-333333333333', 'Monitor 4K', 1200000.00)
ON CONFLICT ("Id") DO NOTHING;