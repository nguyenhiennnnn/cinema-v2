# syntax=docker/dockerfile:1

ARG NODE_VERSION=22-alpine

FROM node:${NODE_VERSION} AS deps
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm ci

FROM deps AS dev
WORKDIR /app
ENV NODE_ENV=development \
    CHOKIDAR_USEPOLLING=true
EXPOSE 5173
CMD ["npm", "run", "dev:frontend"]
