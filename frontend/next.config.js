/** @type {import('next').NextConfig} */
module.exports = {
  reactStrictMode: true,
  experimental: { appDir: true },
  webpack: config => {
    config.resolve.alias['@'] = require('path').resolve(__dirname, 'src');
    return config;
  }
};