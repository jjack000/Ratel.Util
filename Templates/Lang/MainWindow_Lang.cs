using System;
using System.Collections.Generic;
using System.Linq;
namespace NewIP
{
	public class MainWindowLang : LangBase
	{
		public string File => kr ? "파일" : "File";
		public string Language => kr ? "언어 변경" : "Language";
		public string Cascade => kr ? "계단식 구성" : "Cascade";
		public string Tile_Horizontal => kr ? "가로 타일 구성" : "Tile_Horizontal";
		public string Tile_Vertical => kr ? "세로 타일 구성" : "Tile_Vertical";
		public string Arrange_Icons => kr ? "아이콘 정렬" : "Arrange_Icons";
		public string Windows => kr ? "창 설정" : "Windows";
		
	}
}


