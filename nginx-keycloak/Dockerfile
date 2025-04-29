# Use the official Nginx image from Docker Hub
FROM nginx:alpine

# Remove default site config
RUN rm /etc/nginx/conf.d/default.conf

# Copy custom configuration files (if any)
 COPY nginx.conf /etc/nginx/nginx.conf

# Default CMD to run when starting the container
CMD ["nginx", "-g", "daemon off;"]