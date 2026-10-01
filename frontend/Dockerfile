# syntax=docker/dockerfile:1
# Build context: ./frontend. Xem .dockerignore de biet nhung gi bi loai tru
# (node_modules/, dist/, e2e/, test-results/, ...).
#
# Vite 8 dung rolldown va yeu cau Node ^20.19 || >=22.12 — dung Node 22 LTS.
ARG NODE_VERSION=22-alpine

# ───────────────────────── Stage: deps ─────────────────────────
# Cache lop node_modules: chi invalidate khi package.json / lockfile doi.
FROM node:${NODE_VERSION} AS deps
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm ci

# ───────────────────────── Stage: dev ─────────────────────────
# Dung cho docker-compose.yml (development). Source duoc bind-mount tu host de HMR
# hoat dong (xem docker-compose.yml — mount node_modules bang anonymous volume rieng
# de khong bi bind-mount cua host de len).
FROM deps AS dev
WORKDIR /app
ENV NODE_ENV=development \
    CHOKIDAR_USEPOLLING=true
EXPOSE 5173
CMD ["npm", "run", "dev"]
