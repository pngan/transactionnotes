﻿CREATE TABLE IF NOT EXISTS central.user (
    -- Unique identifier for the user using UUID (GUID)
    -- gen_random_uuid() generates a new UUID for each new row
    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    -- User's email address, must be unique and not null
    email VARCHAR(255) UNIQUE NOT NULL,
    first_name VARCHAR(100),
    last_name VARCHAR(100),

    -- Timestamp when the user record was created.
    -- TIMESTAMP WITH TIME ZONE stores the UTC time internally.
    -- DEFAULT NOW() sets the default to the current transaction timestamp (in UTC when using TIMESTAMP WITH TIME ZONE).
    created_at_utc TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,

    -- Timestamp when the user record was last updated.
    -- Can be updated manually or via a trigger.
    updated_at_utc TIMESTAMP WITH TIME ZONE DEFAULT NOW() NOT NULL,
    
    CONSTRAINT unique_email UNIQUE (email)
);

-- Optional: Create an index on the email column for faster lookups
CREATE INDEX idx_user_email ON central.user (email);

-- Function to update updated_at_utc column on update using utc timezone
CREATE OR REPLACE FUNCTION update_updated_at_utc_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at_utc = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Example trigger to use the function
CREATE TRIGGER update_user_updated_at_utc
BEFORE UPDATE ON central.user
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_utc_column();
