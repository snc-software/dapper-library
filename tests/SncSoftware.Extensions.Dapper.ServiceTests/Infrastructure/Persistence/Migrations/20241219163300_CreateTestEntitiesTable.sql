CREATE TABLE IF NOT EXISTS public."TestEntities"
(
    "Id" uuid NOT NULL,
    "Description" text COLLATE pg_catalog."default" NOT NULL,
    "Age" integer NOT NULL,
    "Enabled" boolean NOT NULL,
    CONSTRAINT "PK_TestEntities" PRIMARY KEY ("Id")
)