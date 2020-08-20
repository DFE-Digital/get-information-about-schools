import iebtRadios from "../GiasEdit/iebtRadios";
import GiasAttachUnload from '../GiasModules/GiasModals/GiasAttachUnload';
import GiasTextCounter from '../GiasModules/GiasTextCounter';
iebtRadios.init();

new GiasAttachUnload();
new GiasTextCounter(document.getElementById('Notes'), { maxLength: 4000 });
new GiasTextCounter(document.getElementById('Associations'), { maxLength: 4000 });
