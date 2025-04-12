import 'dotenv/config'
import express from 'express';
import cors from 'cors';
import router from "./authRoutes.js";
import verifyToken from "./authMiddleware.js";
import pool from "./db.js";


const app = express();
app.use(cors());
app.use(express.json())

app.use("/api/auth", router);

app.get("/api/protected", verifyToken, async (req, res) => {
    try{
        const [rows] = await pool.query("SELECT id, username, email FROM users WHERE id = ?", 
            [req.user.userId]);
        const user = rows[0];
        res.json({message: "This is hidden data", user});

    }catch(err){
        console.error(err);
        res.status(500).json({message: "Internal server error"});
    }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () =>{
    console.log(`Server running on port ${PORT}`);
});

