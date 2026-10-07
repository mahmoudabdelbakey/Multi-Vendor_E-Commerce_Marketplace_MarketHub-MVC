import sys, subprocess
STEP_X, STEP_Y = 3.3, 1.7
COL = {'applicant':'#FFF3CD','system':'#E3F2FD','admin':'#E8F5E9','err':'#FDE0DC','neutral':'#F5F5F5','start':'#C8E6C9'}
def render(path, title, nodes, edges, legend=None, w=None):
    """nodes: (id,label,kind,col,row,owner)  kind: start|end|proc|dec|uc|actor"""
    L=['digraph G {','layout=neato; splines=true; overlap=true;',
       'labelloc=t; fontsize=22; fontname="DejaVu Sans"; label="%s";'%title,
       'node [fontname="DejaVu Sans", fontsize=13, style="filled,rounded", shape=box, margin="0.12,0.08"];',
       'edge [fontname="DejaVu Sans", fontsize=11, arrowsize=0.8];']
    for (i,lab,kind,c,r,owner) in nodes:
        pos='pos="%.2f,%.2f!"'%(c*STEP_X, -r*STEP_Y)
        fill=COL.get(owner,'#FFFFFF')
        if kind in('start','end'):
            sh='shape=ellipse, style=filled, fillcolor="%s", width=1.3, height=0.6'%(COL['start'] if kind=='start' else '#E0E0E0')
        elif kind=='dec':
            sh='shape=diamond, style=filled, fillcolor="%s", width=2.5, height=1.25, fontsize=12'%fill
        elif kind=='uc':
            sh='shape=ellipse, style=filled, fillcolor="%s", width=2.5, height=0.7'%fill
        elif kind=='actor':
            sh='shape=box, style="filled,rounded", fillcolor="%s", width=1.6, height=0.7'%fill
        else:
            sh='shape=box, style="filled,rounded", fillcolor="%s", width=2.4, height=0.9'%fill
        L.append('%s [label="%s", %s, %s];'%(i,lab,sh,pos))
    for e in edges: L.append(e)
    if legend:
        L.append('legend [shape=plaintext, style="", fontsize=12, label="%s", pos="%.2f,%.2f!"];'%legend)
    L.append('}')
    open(path+'.dot','w').write("\n".join(L))
