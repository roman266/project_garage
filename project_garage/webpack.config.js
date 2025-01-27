const path = require('path');

module.exports = {
  entry: '.wwwroot/js/App.js', // Твой основной файл
  output: {
    filename: 'bundle.js', // Скомпилированный файл
    path: path.resolve(__dirname, 'dist')
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: ['@babel/preset-react']
          }
        }
      }
    ]
  },
  devServer: {
    static: path.resolve(__dirname, 'dist'), // Обновленная настройка
    port: 3000
  }
};
