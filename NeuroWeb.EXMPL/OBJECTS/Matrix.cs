using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NeuroWeb.EXMPL.OBJECTS {
    /// <summary>
    /// Обьект матрицы
    /// </summary>
    public class Matrix {
        
        /// <summary>
        /// Конструктор матрицы, который принимает уже готовый двумерный массив    
        /// </summary>
        /// <param name="body"> Тело матрицы, двумерный массив </param>
        private Matrix(double[,] body) {
            Row  = body.GetLength(0);
            Col  = body.GetLength(1);
            Body = body;
        }
        
        /// <summary>
        ///  Конструктор матрицы, который принимает только размерности матрицы
        /// </summary>
        /// <param name="row">Строки</param>
        /// <param name="col">Колонки</param>
        public Matrix(int row, int col) {
            Row  = row;
            Col  = col;
            Body = new double[row, col];
        }
        
        private int Row { get; }
        
        private int Col { get; }
        
        public double[,] Body { get; }
        
        /// <summary>
        /// Метод возвращающий транспонированную матрицу
        /// </summary>
        /// <returns> Транспонированная матрица </returns>
        public Matrix GetTranspose() {
            var rows    = Body.GetLength(0);
            var columns = Body.GetLength(1);
            var temp = new double[columns, rows];

            for (var i = 0; i < temp.GetLength(0); i++) {
                for (var j = 0; j < temp.GetLength(1); j++) {
                    temp[i, j] = Body[j, i];
                }
            }

            return new Matrix(temp);
        }
        
        public static double[] operator *(Matrix matrix, double[] neuron) {
            if (matrix.Col != neuron.Length) throw new Exception();
            
            var c = new double[matrix.Row];
            
            for (var x = 0; x < matrix.Row; ++x) {
                double tmp = 0;
                for (var y = 0; y < matrix.Col; ++y) 
                    tmp += matrix.Body[x, y] * neuron[y];
                
                c[x] = tmp;
            }
            
            return c;
        }

        /// <summary>
        /// Метод, заполняющий матрицу случайными числами
        /// </summary>
        public void FillRandom() {
            for (var i = 0; i < Row; i++)
                for (var j = 0; j < Col; j++) {
                    Body[i, j] = new Random().Next() % 100 * 0.03 / (Row + 35);
                }
        }

        /// <summary>
        /// Компанует все значения матрицы в строку
        /// </summary>
        /// <returns> Строка значений </returns>
        [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
        public string GetValues() {
            var tempValues = "";

            for (var i = 0; i < Row; i++) 
                for (var j = 0; j < Col; j++)
                    tempValues += Body[i, j] + " ";
            
            return tempValues;
        }

        public void SetValues(string value, int x, int y) {
            Body[x, y] = double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}