using System;
using System.IO;
using DevExpress.DashboardCommon;

namespace DashboardExporterApp {
    class Program {
        static void Main(string[] args) {
            if(args.Length < 1 || !Directory.Exists(args[0])) {
                Console.WriteLine("Path to the dashboard and output folders are required");
                return;
            }
            string[] dashboards = Directory.GetFiles(args[0], "*.xml");
            string outputFolder = args[1];
            DashboardExporter exporter = new DashboardExporter();
            exporter.ConnectionError += Exporter_ConnectionError;
            exporter.DataLoadingError += Exporter_DataLoadingError;
            exporter.DashboardItemDataLoadingError += Exporter_DashboardItemDataLoadingError;
            foreach(string dashboard in dashboards) {
                string outputFile = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(dashboard)}.pdf");
                using FileStream stream = new FileStream(outputFile, FileMode.OpenOrCreate);
                try {
                    exporter.ExportToPdf(dashboard, stream);
                }
                catch(Exception e) {
                    Console.WriteLine($"Unable to export {dashboard}.");
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
            Console.WriteLine("Done!");
        }
        static void Exporter_ConnectionError(object sender, DashboardExporterConnectionErrorEventArgs e) {
            Console.WriteLine($"The following error occurs in {e.DataSourceName}: {e.Exception.Message}");
        }
        static void Exporter_DataLoadingError(object sender, DataLoadingErrorEventArgs e) {
            foreach(DataLoadingError error in e.Errors)
                Console.WriteLine($"The following error occurs in {error.DataSourceName}: {error.Error}");
        }
        static void Exporter_DashboardItemDataLoadingError(object sender, DashboardItemDataLoadingErrorEventArgs e) {
            foreach(DashboardItemDataLoadingError error in e.Errors)
                Console.WriteLine($"The following error occurs in {error.DashboardItemName}: {error.Error}");
        }
    }
}
