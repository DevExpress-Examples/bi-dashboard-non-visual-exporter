Imports System
Imports System.IO
Imports DevExpress.DashboardCommon

Namespace DashboardExporterApp
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			If args.Length < 1 OrElse Not Directory.Exists(args(0)) Then
				Console.WriteLine("Path to the dashboard and output folders are required")
				Return
			End If
			Dim dashboards() As String = Directory.GetFiles(args(0), "*.xml")
			Dim outputFolder As String = args(1)
			Dim exporter As New DevExpress.DashboardCommon.DashboardExporter()
			AddHandler exporter.ConnectionError, AddressOf Exporter_ConnectionError
			AddHandler exporter.DataLoadingError, AddressOf Exporter_DataLoadingError
			AddHandler exporter.DashboardItemDataLoadingError, AddressOf Exporter_DashboardItemDataLoadingError
			For Each dashboard As String In dashboards
				Dim outputFile As String = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(dashboard)}.pdf")
				Using stream = New FileStream(outputFile, FileMode.OpenOrCreate)
					Try
						exporter.ExportToPdf(dashboard, stream)
					Catch e As Exception
						Console.WriteLine($"Unable to export {dashboard}.")
						Console.WriteLine(e.Message)
						Continue For
					End Try
				End Using
			Next dashboard
			Console.WriteLine("Done!")
		End Sub
		Private Shared Sub Exporter_ConnectionError(ByVal sender As Object, ByVal e As DashboardExporterConnectionErrorEventArgs)
			Console.WriteLine($"The following error occurs in {e.DataSourceName}: {e.Exception.Message}")
		End Sub
		Private Shared Sub Exporter_DataLoadingError(ByVal sender As Object, ByVal e As DataLoadingErrorEventArgs)
			For Each [error] As DataLoadingError In e.Errors
				Console.WriteLine($"The following error occurs in {[error].DataSourceName}: {[error].Error}")
			Next [error]
		End Sub
		Private Shared Sub Exporter_DashboardItemDataLoadingError(ByVal sender As Object, ByVal e As DashboardItemDataLoadingErrorEventArgs)
			For Each [error] As DashboardItemDataLoadingError In e.Errors
				Console.WriteLine($"The following error occurs in {[error].DashboardItemName}: {[error].Error}")
			Next [error]
		End Sub
	End Class
End Namespace
