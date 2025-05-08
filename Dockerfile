# Use the specified Keycloak base image
FROM quay.io/keycloak/keycloak:26.2

# Set environment variables
ENV KC_BOOTSTRAP_ADMIN_USERNAME=admin
ENV KC_BOOTSTRAP_ADMIN_PASSWORD=admin
ENV DB_VENDOR=h2
# set to edge when using https
ENV KC_PROXY=edge
ENV KC_PROXY_HEADERS=xforwarded
ENV KC_HTTP_ENABLED=true
ENV KC_HOSTNAME_STRICT=false
# check this out
ENV KC_HOSTNAME_URL=http://ngrp/keycloak
ENV KC_HOSTNAME=http://ngrp/keycloak
ENV KEYCLOAK_FRONTEND_URL=http://localhost:8080/keycloak
ENV KEYCLOAK_ADMIN_URL=http://localhost:8080/keycloak
ENV KC_HTTP_RELATIVE_PATH=/keycloak
ENV KC_LOG_LEVEL=INFO
ENV KC_HOSTNAME_STRICT_HTTPS=false
ENV KC_HOSTNAME_DEBUG=true
ENV KEYCLOAK_SAMESITE_POLICY=None
ENV PROXY_ADDRESS_FORWARDING=true
ENV KEYCLOAK_COOKIE_DOMAIN=localhost


# Expose the necessary ports
EXPOSE 8080

# Set the volume mount path
VOLUME /opt/keycloak/data

# Define the entrypoint to start Keycloak in dev mode
CMD ["start-dev", "--http-port", "8080"]