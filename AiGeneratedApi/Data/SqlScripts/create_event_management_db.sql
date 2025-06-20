-- Create Users table (simplified for ASP.NET Identity)
CREATE TABLE IF NOT EXISTS Users (
    Id TEXT PRIMARY KEY,
    UserName TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    RefreshToken TEXT,
    RefreshTokenExpiryTime TEXT
    -- Add other ASP.NET Identity fields as needed
);

-- Create Events table
CREATE TABLE IF NOT EXISTS Events (
    Id TEXT PRIMARY KEY,
    Title TEXT NOT NULL,
    Description TEXT,
    Date TEXT NOT NULL,
    Location TEXT NOT NULL,
    ImageUrl TEXT,
    OwnerId TEXT NOT NULL,
    FOREIGN KEY (OwnerId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create EventRegistrations table
CREATE TABLE IF NOT EXISTS EventRegistrations (
    Id TEXT PRIMARY KEY,
    EventId TEXT NOT NULL,
    UserId TEXT NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    UNIQUE (EventId, UserId) -- Prevent duplicate registrations
); 