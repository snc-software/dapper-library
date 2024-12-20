CREATE TABLE IF NOT EXISTS public."AttributeEntities"
(
    "Id" uuid NOT NULL,
    "Description" text COLLATE pg_catalog."default" NOT NULL,
    "Age" integer NOT NULL,
    "Enabled" boolean NOT NULL,
    CONSTRAINT "PK_AttributeEntities" PRIMARY KEY ("Id")
)