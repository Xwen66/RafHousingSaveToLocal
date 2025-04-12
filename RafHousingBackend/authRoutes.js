
import express from 'express';
import bcrypt from "bcrypt";
import pool from "./db.js"; // Importing the database connection pool
import jwt from "jsonwebtoken"
const router = express.Router();
// const jwt = require("jsonwebtoken");
// const bcrypt = require("bcrypt");
// Route for user signup
router.post("/signup", async (req, res) => {
    try {
        const { username, email, password } = req.body;

        // Validate the input
        if (!username || !email || !password) {
            return res.status(400).json({ message: "Missing username/email/password" });
        }

        // Check if the user already exists
        const [existing] = await pool.query(
            "SELECT id FROM users WHERE username = ? OR email = ?",
            [username, email]
        );
        if (existing.length > 0) {
            return res.status(400).json({ message: "User or email already exists" });
        }

        // Hash the password
        const saltRounds = 10;
        const hashedPassword = await bcrypt.hash(password, saltRounds);

        // Insert the new user into the database
        await pool.query(
            "INSERT INTO users (username, email, password_hash) VALUES (?, ?, ?)",
            [username, email, hashedPassword]
        );

        res.status(200).json({ message: "User created successfully." });
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: "Internal server error" });
    }
});

// Route for user signin
router.post("/signin", async (req, res) => {
    try {
        const { username, password } = req.body;

        // Validate the input
        if (!username || !password) {
            return res.status(400).json({ message: "Missing username/password" });
        }

        // Check if the user exists
        const [existing] = await pool.query(
            "SELECT id, username, password_hash FROM users WHERE username = ?",
            [username]
        );

        if (existing.length === 0) {
            return res.status(401).json({ message: "Invalid credentials" });
        }

        const user = existing[0];

        // Compare the provided password with the stored hash
        const match = await bcrypt.compare(password, user.password_hash);
        if (!match) {
            return res.status(401).json({ message: "Invalid credentials" });
        }

        // Generate a JWT token
        const token = jwt.sign(
            { userId: user.id, username: user.username },
            process.env.JWT_SECRET,
            { expiresIn: "1d" }
        );

        return res.status(200).json({ message: "Logged in", token });
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: "Internal server error" });
    }
});

export default router; // Export the router