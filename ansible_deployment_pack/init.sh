# Init env to run ansible playbooks on IAC servers



# Install known certificates

# yum -y install ca-certificates;
# update-ca-trust force-enable;

echo cp templates/iac-root-ca.crt /etc/pki/ca-trust/source/anchors/;
cp templates/iac-root-ca.crt /etc/pki/ca-trust/source/anchors/;                     # iac root certificate 

echo cp templates/iac-root-intermediate.crt /etc/pki/ca-trust/source/anchors/;
cp templates/iac-root-intermediate.crt /etc/pki/ca-trust/source/anchors/;           # iac intermediate certificate

echo cp templates/qazcertMITM.crt /etc/pki/ca-trust/source/anchors/;
cp templates/qazcertMITM.crt /etc/pki/ca-trust/source/anchors/;                     # qazcert certificate

echo update-ca-trust;
update-ca-trust;                                                                    # update known certificates



# Install ansible 
echo yum -y -q install ansible;
yum -y -q install ansible;

echo Init completed;