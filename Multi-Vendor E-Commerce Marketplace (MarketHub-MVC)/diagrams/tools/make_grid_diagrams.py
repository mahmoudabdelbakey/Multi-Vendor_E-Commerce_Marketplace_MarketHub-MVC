import sys; sys.path.insert(0,'tools')
from grid import render
S='src/'
# 11 Vendor registration
N=[('s','Start','start',0,0,'applicant'),
('a1','Open Vendor\\nApplication form','proc',1,0,'applicant'),
('a2','Fill account +\\nstore details','proc',2,0,'applicant'),
('d1','Valid and\\nunique?','dec',3,0,'system'),
('s2','Create ApplicationUser\\n(no Vendor role yet)','proc',4,0,'system'),
('s3','Create VendorProfile\\nStatus = Pending','proc',4,1,'system'),
('s4','Show: application\\nunder review','proc',3,1,'system'),
('ad1','Admin opens\\nPending Applications','proc',2,1,'admin'),
('ad2','Approve or\\nReject?','dec',1,1,'admin'),
('ap','Approve: add Vendor role,\\nStatus = Approved,\\nupdate SecurityStamp','proc',0.6,2.3,'system'),
('rj','Reject: Status = Rejected,\\nsave RejectionReason,\\nno Vendor role','proc',2.4,2.3,'system'),
('e','End','end',1.5,3.4,'neutral')]
E=['s->a1','a1->a2','a2->d1','d1->s2 [label="Yes"]','d1->a2 [label="No (show errors)", constraint=false]','s2->s3','s3->s4','s4->ad1','ad1->ad2','ad2->ap [label="Approve"]','ad2->rj [label="Reject"]','ap->e','rj->e']
render(S+'11_vendor_registration_activity','Vendor Registration and Approval - Activity Diagram',N,E,('Legend:  yellow = Applicant   blue = System   green = Super Admin',1.5,-4.1))
# 12 product creation
N=[('s','Vendor opens\\nCreate Product','start',0,0,'applicant'),
('d1','Approved\\nVendor?','dec',1,0,'system'),
('f','Fill ProductViewModel\\nName, Description, Price,\\nStock, Category, Images','proc',2,0,'applicant'),
('p','POST\\n/Vendor/Products/Create','proc',3,0,'applicant'),
('d2','Fields\\nvalid?','dec',4,0,'system'),
('x','403 Forbidden or\\nredirect to Login','proc',1,-1.2,'err'),
('d3','Images valid?\\next, size, type','dec',4,1.2,'system'),
('sv','Service sets VendorProfileId\\nfrom current user (never\\nfrom the form), saves\\nProduct + ProductImages','proc',3,1.2,'system'),
('e','Redirect to My Products\\nwith success message','end',2,1.2,'neutral')]
E=['s->d1','d1->f [label="Yes"]','d1->x [label="No"]','f->p','p->d2','d2->d3 [label="Yes"]','d2->f [label="No", constraint=false]','d3->sv [label="Yes"]','d3->f [label="No", constraint=false]','sv->e']
render(S+'12_product_creation_activity','Product Creation - Activity Diagram',N,E,('Legend:  yellow = Vendor   blue = System   red = error path',2,2.5))
# 13 cart
N=[('s','Customer clicks\\nAdd to Cart','start',0,0,'applicant'),
('d1','Logged in?','dec',1,0,'system'),
('ld','CartService loads Product\\nfrom DB (price + stock)','proc',2,0,'system'),
('d2','Active product,\\nvendor Approved?','dec',3,0,'system'),
('d3','Quantity valid and\\nwithin Stock?','dec',4,0,'system'),
('lg','Redirect to Login,\\nthen return','proc',1,-1.2,'err'),
('e1','Show: product\\nunavailable','proc',3,-1.2,'err'),
('e2','Show: not enough\\nstock','proc',4,-1.2,'err'),
('up','Insert or update CartItem\\n(unique CustomerId+ProductId)','proc',4,1.2,'system'),
('vw','Cart page recalculates\\ntotals from DB prices','proc',3,1.2,'system'),
('d4','Customer\\naction?','dec',2,1.2,'applicant'),
('rm','Delete CartItem','proc',2,2.5,'system'),
('c','Go to Checkout','end',1,1.2,'neutral')]
E=['s->d1','d1->ld [label="Yes"]','d1->lg [label="No"]','lg->ld [constraint=false]','ld->d2','d2->d3 [label="Yes"]','d2->e1 [label="No"]','d3->up [label="Yes"]','d3->e2 [label="No"]','up->vw','vw->d4','d4->c [label="Checkout"]','d4->rm [label="Remove"]','rm->vw [constraint=false]','d4->d3 [label="Update qty", constraint=false]']
render(S+'13_cart_activity','Shopping Cart - Activity Diagram',N,E,('Legend:  yellow = Customer   blue = System   red = error path',2,3.4))
# 14 checkout
N=[('s','Customer clicks\\nCheckout','start',0,0,'applicant'),
('d1','Cart not\\nempty?','dec',1,0,'system'),
('ad','Enter shipping\\ndetails','proc',2,0,'applicant'),
('po','POST /Checkout/Pay\\n(AntiForgery token)','proc',3,0,'applicant'),
('d2','Reload products from DB:\\nall valid + in stock?','dec',4,0,'system'),
('e0','Redirect to Cart','proc',1,-1.2,'err'),
('fx','Show problems,\\nupdate cart','proc',4,-1.2,'err'),
('tx','BEGIN TRANSACTION:\\ncreate Order + OrderItems,\\nreserve stock,\\ncreate Payment Pending','proc',4,1.3,'system'),
('d3','Stock reserved\\nfor all items?','dec',3,1.3,'system'),
('rb','ROLLBACK, show\\ninsufficient stock','proc',3,2.6,'err'),
('cm','COMMIT, then create\\nStripe Checkout Session\\n(metadata OrderId, expires_at)','proc',2,1.3,'system'),
('e','Redirect to Stripe-\\nhosted payment page','end',1,1.3,'neutral')]
E=['s->d1','d1->ad [label="Yes"]','d1->e0 [label="No"]','ad->po','po->d2','d2->tx [label="Yes"]','d2->fx [label="No"]','tx->d3','d3->cm [label="Yes"]','d3->rb [label="No"]','cm->e']
render(S+'14_checkout_activity','Checkout - Activity Diagram',N,E,('Legend:  yellow = Customer   blue = System   red = error path',2,3.5))
# 22 vendor isolation
N=[('r','POST /Vendor/Products/Edit/17','start',0,0,'applicant'),
('a','Authenticated?','dec',1,0,'system'),
('p','Policy ApprovedVendor:\\nrole Vendor AND\\nStatus = Approved?','dec',2,0,'system'),
('v','ICurrentVendorService\\nresolves VendorProfileId','proc',3,0,'system'),
('q','Query Products WHERE Id = 17\\nAND VendorProfileId = currentVendorId','proc',3,1.3,'system'),
('f','Row found?','dec',2,1.3,'system'),
('ok','Bind ONLY allowed\\nViewModel fields, save','end',1,1.3,'neutral'),
('r1','Challenge:\\nredirect to Login','proc',1,-1.2,'err'),
('r2','403 Forbidden','proc',2,-1.2,'err'),
('r3','404 Not Found\\n(same for missing and\\nother vendor product)','proc',2,2.7,'err')]
E=['r->a','a->p [label="Yes"]','a->r1 [label="No"]','p->v [label="Yes"]','p->r2 [label="No"]','v->q','q->f','f->ok [label="Yes"]','f->r3 [label="No"]']
render(S+'22_vendor_isolation','Vendor Isolation - Authorization Flow',N,E)
# 18 github workflow (snake)
N=[('i','GitHub Issue\\n(story slice)','start',0,0,'admin'),
('b','git switch -c\\nfeature/42-name','proc',1,0,'system'),
('c','Small commits\\n(pair programming)','proc',2,0,'system'),
('sy','git fetch + rebase\\nor merge origin/main','proc',3,0,'system'),
('pu','git push -u origin\\nfeature/...','proc',4,0,'system'),
('pr','Open Pull Request\\n(template, Closes #42)','proc',4,1.5,'applicant'),
('ci','CI: build + tests\\npass?','dec',3,1.5,'system'),
('rv','Peer review by\\nassigned reviewer','proc',2,1.5,'applicant'),
('ch','Changes\\nrequested?','dec',1,1.5,'applicant'),
('ap','Approve (1 reviewer,\\n2 for DB / Payment)','proc',1,3,'admin'),
('mg','Squash and merge\\ninto main','proc',2,3,'admin'),
('dl','Delete branch,\\nall pull main','end',3,3,'neutral')]
E=['i->b','b->c','c->sy','sy->pu','pu->pr','pr->ci','ci->rv [label="Yes"]','ci->c [label="No: fix", constraint=false, style=dashed]','rv->ch','ch->ap [label="No"]','ch->c [label="Yes: fix", constraint=false, style=dashed]','ap->mg','mg->dl']
render(S+'18_github_workflow','GitHub Collaboration Workflow',N,E)
# 19 lifecycle (two rows)
N=[('a','Product Backlog\\n(user stories)','proc',0,0,'admin'),
('b','Shared Sprint\\nPlanning','proc',1,0,'applicant'),
('c','Shared Design Session\\nrequirements, ERD, UI','proc',2,0,'applicant'),
('d','Implement vertical slice\\nDB, Service, Controller, View','proc',3,0,'system'),
('e','Collective Testing\\nunit, integration, manual','proc',4,0,'system'),
('f','Cross-member\\nCode Review','proc',4,1.6,'applicant'),
('g','Integration into main\\n+ regression run','proc',3,1.6,'system'),
('h','Sprint Review\\n(demo)','proc',2,1.6,'admin'),
('i','Retrospective','proc',1,1.6,'admin'),
('j','Deploy to demo\\nenvironment (Sprint 9)','end',3,3.0,'neutral')]
E=['a->b','b->c','c->d','d->e','e->f','f->g','g->h','h->i','i->a [label="next Sprint"]','g->j [style=dashed]']
render(S+'19_development_lifecycle','Development Lifecycle (Sprint Loop)',N,E)
# 02 overall use cases (grouped boxes)
from grid import STEP_X, STEP_Y
UC={'01':'UC-01 Browse Products','02':'UC-02 Search, Filter, Sort','03':'UC-03 View Product Details','04':'UC-04 Register Account','05':'UC-05 Login / Logout','06':'UC-06 Apply as Vendor',
'07':'UC-07 Manage Cart','08':'UC-08 Checkout','09':'UC-09 Pay with Stripe','10':'UC-10 View Order History','11':'UC-11 Cancel Pending Order','12':'UC-12 Write Review','13':'UC-13 Manage Profile',
'14':'UC-14 Store Profile','15':'UC-15 Manage Products','16':'UC-16 Product Images','17':'UC-17 Manage Inventory','18':'UC-18 View My Order Items','19':'UC-19 Update Fulfillment','20':'UC-20 Sales Summary','21':'UC-21 Commission Summary','22':'UC-22 Vendor Dashboard',
'23':'UC-23 Review Vendor Apps','24':'UC-24 Suspend Vendor','25':'UC-25 Manage Categories','26':'UC-26 Configure Commission','27':'UC-27 Monitor All Orders','28':'UC-28 Platform Reports','29':'UC-29 Moderate Products','31':'UC-31 Stripe Webhook'}
RS=0.50
L=['digraph G {','layout=neato; splines=line; overlap=true;','labelloc=t; fontsize=24; fontname="DejaVu Sans"; label="Overall UML Use Case Diagram";',
 'node [fontname="DejaVu Sans", fontsize=13]; edge [fontname="DejaVu Sans", fontsize=12, color="#555555"];']
groups=[('Guest package','Guest',1.0,['01','02','03','04','05','06'],'#FFF8E1','#FFF3CD'),
        ('Customer package','Customer',2.15,['07','08','09','10','11','12','13'],'#FFF8E1','#FFF3CD'),
        ('Vendor package','Vendor',3.3,['14','15','16','17','18','19','20','21','22'],'#EAF4FD','#E3F2FD'),
        ('Super Admin package','Admin',4.45,['23','24','25','26','27','28','29'],'#F1F8F1','#E8F5E9')]
maxn=9
top=-1.2
for gl,aid,c,ids,bg,fill in groups:
    n=len(ids); h=n*RS*STEP_Y+0.9
    cy=-(( (n-1)/2.0)*RS)*STEP_Y
    L.append('g%s [shape=box, style="dashed,filled", fillcolor="%s", label="%s", labelloc=t, fontsize=14, width=3.1, height=%.2f, fixedsize=true, pos="%.2f,%.2f!"];'%(aid,bg,gl,h,c*STEP_X,cy))
    for k,i in enumerate(ids):
        L.append('u%s [shape=ellipse, style=filled, fillcolor="%s", label="%s", width=2.7, height=0.62, fixedsize=true, pos="%.2f,%.2f!"];'%(i,fill,UC[i],c*STEP_X,-(k*RS+0.15)*STEP_Y))
    L.append('%s [shape=box, style="filled,rounded", fillcolor="%s", label="«actor»\\n%s", width=1.6, height=0.7, pos="%.2f,%.2f!"];'%(aid,fill,'Super Admin' if aid=='Admin' else aid,c*STEP_X,1.7))
    L.append('%s -> g%s [arrowhead=none, penwidth=2];'%(aid,aid))
L.append('gSys [shape=box, style="dashed,filled", fillcolor="#F5F5F5", label="System package", labelloc=t, fontsize=14, width=3.3, height=1.6, fixedsize=true, pos="%.2f,%.2f!"];'%(5.6*STEP_X,-0.2*STEP_Y))
L.append('u31 [shape=ellipse, style=filled, fillcolor="#EEEEEE", label="%s", width=2.7, height=0.62, fixedsize=true, pos="%.2f,%.2f!"];'%(UC['31'],5.6*STEP_X,-0.25*STEP_Y))
L.append('Stripe [shape=box, style="filled,rounded", fillcolor="#EEEEEE", label="«actor»\\nStripe (external)", width=1.9, height=0.7, pos="%.2f,%.2f!"];'%(5.6*STEP_X,1.7))
L.append('Stripe -> gSys [arrowhead=none, penwidth=2];')
L.append('Customer -> Guest [arrowhead=empty, style=dashed, label="inherits"];')
L.append('lg [shape=plaintext, style="", fontsize=12, label="Each actor is linked to its package: it may perform every use case inside it (details in Diagrams 3, 4, 5)", pos="%.2f,%.2f!"];'%(3.0*STEP_X,-8.3))
L.append('}')
open(S+'02_usecase_overall.dot','w').write("\n".join(L))
