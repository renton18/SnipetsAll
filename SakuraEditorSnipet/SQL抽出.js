var cnt = GetLineCount(0);
var textAll = "";
for (var i = 1; i <= cnt; i++) {
    // 行の文字列を取得
    var str = GetLineStr(i).replace(/\r\n/,"").replace(/\n/,"");
    // コード削除
    str = str.replace(/^.*?\"/,"");		// 行頭から最初のダブルクォーテーションまでを削除
	str = str.replace(/\".*?$/, "");	// 行末から最初のダブルクォーテーションまでを削除

    str = str.replace(/ +$/,"");		// 行頭の空白削除
    str = str.replace(/^ /,"");			// 行末の空白削除    
    
    str = str.replace(/.:.*?[ ]/g," '' ");		// :を''に入れ替え
    str = str.replace(/.:.*?$/," '' ");			// :を''に入れ替え
    
    textAll += str + "\n";
}
SelectAll(0);
InsText(textAll);
