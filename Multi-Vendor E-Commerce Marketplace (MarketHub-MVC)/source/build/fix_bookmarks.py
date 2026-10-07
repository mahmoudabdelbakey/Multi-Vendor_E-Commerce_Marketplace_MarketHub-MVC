import zipfile, re, sys, shutil
src=sys.argv[1]; tmp=src+'.tmp'
zin=zipfile.ZipFile(src); zout=zipfile.ZipFile(tmp,'w',zipfile.ZIP_DEFLATED)
for item in zin.infolist():
    data=zin.read(item.filename)
    if item.filename=='word/document.xml':
        x=data.decode('utf8'); n=[0]
        def rep(m):
            if m.group(1)=='Start':
                n[0]+=1
            return re.sub(r'w:id="\d+"','w:id="%d"'%(n[0]+1000),m.group(0))
        x=re.sub(r'<w:bookmark(Start|End)\b[^>]*>',rep,x)
        data=x.encode('utf8')
    zout.writestr(item,data)
zout.close(); shutil.move(tmp,src)
