var cnt = GetLineCount(0);
var textAll = "";
for (var i = 1; i <= cnt; i++) {
    // �s�̕�������擾
    var str = GetLineStr(i).replace(/\r\n/,"").replace(/\n/,"");
    // �R�[�h�폜
    str = str.replace(/^.*?\"/,"");		// �s������ŏ��̃_�u���N�H�[�e�[�V�����܂ł��폜
	str = str.replace(/\".*?$/, "");	// �s������ŏ��̃_�u���N�H�[�e�[�V�����܂ł��폜

    str = str.replace(/ +$/,"");		// �s���̋󔒍폜
    str = str.replace(/^ /,"");			// �s���̋󔒍폜    
    
    str = str.replace(/.:.*?[ ]/g," '' ");		// :��''�ɓ���ւ�
    str = str.replace(/.:.*?$/," '' ");			// :��''�ɓ���ւ�
    
    textAll += str + "\n";
}
SelectAll(0);
InsText(textAll);
