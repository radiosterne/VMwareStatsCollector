using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VMwareStatsCollector.Entities;

namespace VMwareStatsCollector.Services
{
	public sealed class ExcelPrintingService
	{
		public void PrintHostStats(IReadOnlyCollection<Host> hostStats)
		{
			using (var p = new ExcelPackage())
			{
				var sheet = p.Workbook.Worksheets.Add("Hosts");
				var currentRow = 1;
				foreach (var host in hostStats)
				{
					var firstHostRow = currentRow;
					sheet.Cells[currentRow, 1].Value = "Host";
					sheet.Cells[currentRow, 2].Value = host.Name;
					sheet.Cells[currentRow, 1].Style.Font.Bold = true;
					currentRow++;
					PrintHostHeader(currentRow, sheet);
					currentRow++;
					sheet.Cells[currentRow, 1].Value = host.CpuModel;
					sheet.Cells[currentRow, 2].Value = host.CoresCount;
					sheet.Cells[currentRow, 3].Value = host.RamVolume;
					sheet.Cells[currentRow, 4].Value = host.UsedCoresCount;
					sheet.Cells[currentRow, 5].Value = host.ConsumedCpuPercentage;
					sheet.Cells[currentRow, 6].Value = host.ConsumedMemoryPercentage;
					currentRow += 2;

					PrintVMHeader(currentRow, sheet);
					currentRow++;
					foreach (var vm in host.VirtualMachines)
					{
						sheet.Cells[currentRow, 1].Value = vm.Name;
						sheet.Cells[currentRow, 2].Value = vm.CoresCount;
						sheet.Cells[currentRow, 3].Value = vm.RamVolume;
						sheet.Cells[currentRow, 4].Value = vm.ConsumedCpuPercentage;
						sheet.Cells[currentRow, 5].Value = vm.ConsumedMemoryPercentage;
						sheet.Cells[currentRow, 6].Value = vm.MaxConsumedCpuPercentage;
						sheet.Cells[currentRow, 7].Value = vm.MaxConsumedMemoryPercentage;
						currentRow++;
					}

					sheet.Cells[firstHostRow, 1, currentRow - 1, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

					currentRow++;
				}

				sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
				p.SaveAs(new FileInfo(@"hosts.xlsx"));
			}
		}

		private static void PrintHostHeader(int row, ExcelWorksheet sheet)
		{
			sheet.Cells[row, 1].Value = "CPU model";
			sheet.Cells[row, 2].Value = "Cores";
			sheet.Cells[row, 3].Value = "RAM, MB";
			sheet.Cells[row, 4].Value = "Used cores";
			sheet.Cells[row, 5].Value = "Used CPU, %";
			sheet.Cells[row, 6].Value = "Used RAM, %";
			sheet.Cells[row, 1, row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
			sheet.Cells[row, 1, row, 6].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
		}

		private static void PrintVMHeader(int row, ExcelWorksheet sheet)
		{
			sheet.Cells[row, 1].Value = "Name";
			sheet.Cells[row, 2].Value = "Cores";
			sheet.Cells[row, 3].Value = "RAM, MB";
			sheet.Cells[row, 4].Value = "Used CPU, %";
			sheet.Cells[row, 5].Value = "Used RAM, %";
			sheet.Cells[row, 6].Value = "Max used CPU, %";
			sheet.Cells[row, 7].Value = "Max used RAM, %";
			sheet.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
			sheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
		}
	}
}
