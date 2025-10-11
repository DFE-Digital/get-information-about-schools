import iebtRadios from "../GiasEdit/iebtRadios";
import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
import GiasTextCounter from '../GiasModules/GiasTextCounter';
iebtRadios.init();

new GiasAttachUnload();
if (document.getElementById('Notes')){
  new GiasTextCounter(document.getElementById('Notes'), { maxLength: 4000 });
}

if (document.getElementById('Associations')){
  new GiasTextCounter(document.getElementById('Associations'), { maxLength: 1000 });
}

