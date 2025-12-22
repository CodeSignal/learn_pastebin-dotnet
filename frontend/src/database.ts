import Database from 'better-sqlite3';

const DB_PATH = './database.sqlite';
const db = new Database(DB_PATH); // This opens the DB synchronously

export default db;
