CREATE TABLE [User] (
    UserId BIGINT IDENTITY PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    IsAdmin BIT NOT NULL DEFAULT 0,
    IsLocked BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Category (
    CategoryId BIGINT IDENTITY PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Difficulty (
    DifficultyId BIGINT IDENTITY PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Ingredient (
    IngredientId BIGINT IDENTITY PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Recipe (
    RecipeId BIGINT IDENTITY PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    PreparationMethod TEXT NOT NULL,
    PreparationTime INT NOT NULL,
    CategoryId BIGINT NOT NULL,
    DifficultyId BIGINT NOT NULL,
    CreatedByUserId BIGINT NOT NULL,
    IsApproved BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Recipe_Category FOREIGN KEY (CategoryId)
        REFERENCES Category(CategoryId),

    CONSTRAINT FK_Recipe_Difficulty FOREIGN KEY (DifficultyId)
        REFERENCES Difficulty(DifficultyId),

    CONSTRAINT FK_Recipe_User FOREIGN KEY (CreatedByUserId)
        REFERENCES [User](UserId)
);

CREATE TABLE RecipeIngredient (
    RecipeId BIGINT NOT NULL,
    IngredientId BIGINT NOT NULL,
    Quantity VARCHAR(50),
    Unit VARCHAR(50),

    CONSTRAINT PK_RecipeIngredient PRIMARY KEY (RecipeId, IngredientId),

    CONSTRAINT FK_RI_Recipe FOREIGN KEY (RecipeId)
        REFERENCES Recipe(RecipeId)
        ON DELETE CASCADE,

    CONSTRAINT FK_RI_Ingredient FOREIGN KEY (IngredientId)
        REFERENCES Ingredient(IngredientId)
);

CREATE TABLE Comment (
    CommentId BIGINT IDENTITY PRIMARY KEY,
    RecipeId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,
    Text TEXT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Comment_Recipe FOREIGN KEY (RecipeId)
        REFERENCES Recipe(RecipeId)
        ON DELETE CASCADE,

    CONSTRAINT FK_Comment_User FOREIGN KEY (UserId)
        REFERENCES [User](UserId)
);

CREATE TABLE Rating (
    RatingId BIGINT IDENTITY PRIMARY KEY,
    RecipeId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,
    Value INT NOT NULL CHECK (Value BETWEEN 1 AND 5),

    CONSTRAINT FK_Rating_Recipe FOREIGN KEY (RecipeId)
        REFERENCES Recipe(RecipeId)
        ON DELETE CASCADE,

    CONSTRAINT FK_Rating_User FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT UQ_Rating UNIQUE (RecipeId, UserId)
);

CREATE TABLE Favourite (
    UserId BIGINT NOT NULL,
    RecipeId BIGINT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_Favourite PRIMARY KEY (UserId, RecipeId),

    CONSTRAINT FK_Fav_User FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT FK_Fav_Recipe FOREIGN KEY (RecipeId)
        REFERENCES Recipe(RecipeId)
        ON DELETE CASCADE
);

