FROM plugfox/flutter:stable-web as build
WORKDIR /app
COPY . .
# Fetch the dependencies
RUN flutter pub get
# Build the application
RUN flutter build web

## Use Nginx to serve the app
FROM nginx:alpine

# Copy the build artifacts from the build stage, and the Nginx configuration
COPY --from=build /app/build/web /usr/share/nginx/html
COPY docker/web/nginx.conf.template /etc/nginx/templates/default.conf.template

EXPOSE $PORT

CMD ["nginx", "-g", "daemon off;"]