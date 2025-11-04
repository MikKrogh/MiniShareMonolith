# Makefile for Docker and Postgres tasks

# Docker image configuration
IMAGE_NAME=ghcr.io/mikkrogh/minishare
IMAGE_TAG=latest

# Postgres configuration
DB_CONTAINER_NAME=postgres16
DB_IMAGE=postgres:16
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=MyPreciousPassword123!

.PHONY: all build push db-start db-stop db-reset db-shell help

all: help

# Help message
help:
	@echo "Available commands:"
	@echo "  make build        - Build the Docker image"
	@echo "  make push         - Push Docker image to registry"
	@echo "  make db-start     - Start the Postgres container"
	@echo "  make db-stop      - Stop and remove the Postgres container"
	@echo "  make db-reset     - Reset the Postgres container"
	@echo "  make db-shell     - Open a psql shell to the Postgres container"

# Build Docker image
build:
	docker build -t $(IMAGE_NAME):$(IMAGE_TAG) .

# Push Docker image
push:
	docker push $(IMAGE_NAME):$(IMAGE_TAG)

# Start Postgres container
db-start:
	docker run -d \
		--name $(DB_CONTAINER_NAME) \
		-e POSTGRES_USER=$(DB_USER) \
		-e POSTGRES_PASSWORD=$(DB_PASSWORD) \
		-p $(DB_PORT):5432 \
		$(DB_IMAGE)

# Stop Postgres container
db-stop:
	docker stop $(DB_CONTAINER_NAME) || true
	docker rm $(DB_CONTAINER_NAME) || true

# Reset Postgres container
db-reset: db-stop db-start
