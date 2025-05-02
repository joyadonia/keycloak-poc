# Use the specified Keycloak base image
FROM quay.io/keycloak/keycloak:26.2

# Set environment variables
ENV KC_BOOTSTRAP_ADMIN_USERNAME=admin
ENV KC_BOOTSTRAP_ADMIN_PASSWORD=admin
ENV DB_VENDOR=h2
ENV KC_PROXY=edge
ENV KC_PROXY_HEADERS=xforwarded
ENV KC_HTTP_ENABLED=true
ENV KC_HOSTNAME_STRICT=false
ENV KC_LOG_LEVEL=INFO


# Expose the necessary ports
EXPOSE 8080

# Set the volume mount path
VOLUME /opt/keycloak/data

# Define the entrypoint to start Keycloak in dev mode
CMD ["start-dev", "--http-port", "8080"]