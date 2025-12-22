# Use Node.js as base image
FROM node:20-slim

# Install build dependencies for sqlite3
RUN apt-get update
RUN apt-get install -y \
    python3 \
    make \
    g++ \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Build the frontend
RUN npm run build

# Build the server
RUN npm run build:server

# Rebuild sqlite3 from source
RUN npm rebuild sqlite3 --build-from-source

# Expose port
EXPOSE 3000

# Start the server
CMD ["npm", "run", "start"]
