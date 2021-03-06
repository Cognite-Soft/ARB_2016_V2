/*
	             __    
	.---.-.----.|  |--.
	|  _  |   _||  _  |
	|___._|__|  |_____|

	Author    : Andy Proios
	Version   : 0.1

*/

/* Init
--------------------------------------*/
$(document).foundation();


/*Cognite*/

$(document).ready(function () {

    /*Pravahya
    Event Binders for Edit button - Cancel Button 
    */
    $("a[name='editBtn']").each(function () {/*add behaviour to the edit buttons such that taht have a relationship with the textarea.*/
        $(this).bind('click', function () {/*When the edit button is clicked, the appropriate text area is activated and no other....*/
            $(this).parent().children("#txtAreaDiscussion").attr('readonly', false);
            var textAreaValue = $(this).parent().children("#txtAreaDiscussion").val();
            $(this).siblings("input[name='txtAreaDiscussionHidden']").val(textAreaValue);
        });
    });

    $("a[name='cancelBtn']").each(function () {
        $(this).bind('click', function () {/*We need to set the original comment back, not just disabling the comment wont do the task*/
            $(this).parent().children("#txtAreaDiscussion").attr('readonly', true);
            var textAreaValue = $(this).siblings("input[name='txtAreaDiscussionHidden']").val();
            $(this).parent().children("#txtAreaDiscussion").val(textAreaValue);

        });
    });
    /*Pravahya
    Event Binders for Edit button - Cancel Button 
    */
    
    

});




$('.button tiny yes reply-button').on('click', function (e) {

    //var selectedElement=jQuery(this).find("txtAreaDiscussion");
    //    alert("my text");
    //    selectedElement.prop('readonly', false);


    
});


//function onEditReadOnlyClick() {
//    alert("my text");
//    $("#txtAreaDiscussion").prop('readonly', false);
//}


/*Cognite*/

$(function () {
    ShowMessage();
    BindCaseOptionChange();
    BindAttchments();
    //BindDecisionDocument();
});

function ShowMessage() {
    var files = $('#readme > ul.no-bullet > li');
    if (files.length > 0 && window.location.href.indexOf('#') == -1) {
        $(document).foundation('joyride', 'start');
    }
}


/* Mobile check and set event type
--------------------------------------*/
function mobilecheck() {
  var check = false;
  (function (a) { if (/(android|ipad|playbook|silk|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true })(navigator.userAgent || navigator.vendor || window.opera);
  return check;
}

var eventtype = mobilecheck() ? 'touchstart' : 'click';


/* Show hide functions
--------------------------------------*/
//add new
$('body').on(eventtype, '.add', function () {
  $('#addNew').toggleClass('hide');
});

//comments
$('body').on(eventtype, '.additional', function(){
  var getID = $(this).attr('data-show');
  $('#' + getID + ' .' + getID).toggleClass('hide');
});

//expand
$('body').on(eventtype, '.expand', function(){
    var getID = $(this).attr('data-show');
    $('#'+getID).toggleClass('hide');
    $(this).children('i').toggleClass('fa-caret-square-o-up');
});

//new post scroll
$(function () {
    $('.new-post').click(function () {
        $('#newpost').toggleClass('hide');
        $("html, body").animate({ scrollTop: $(document).height() }, 300);
        $(".new-post-area").focus();
    });
});

function BindCaseOptionChange() {
    $("#switch-case").change(CaseOptionChange);
}

function CaseOptionChange() {
    var selected = $("#switch-case option:selected");
    var url = selected.attr("value");
    window.location.href = url;
}

// attachment
function BindAttchments() {
    $('a.attach').click(AddAttachmentClick);
    $('input[type="file"]').change(FileSelected);
}

function AddAttachmentClick(e) {
    var sender = $(e.target);    
    var inputId = sender.attr('data-input-id');
    
    if (typeof inputId === 'undefined') //getting parent tag id if <i> tag is on click    
    {
        var inputId = sender.parent().attr('data-input-id');
    }

    var input = $('#' + inputId);
    //alert (inputId);
    input.click();
}

function FileSelected(e) {
    var sender = $(e.target);
    var val = sender.val();
    var fileName = val.substring(val.lastIndexOf('\\') + 1, val.length);
    var id = sender.attr('id');
    var span = $('span.' + id);
    span.html(fileName);
}

function printPage() {
    //Print Page
    window.print();

}

//Pravahya
//validate start process
//Pravahya
//Check is allegations are present when the complaints are saved.
function validateSave(){
    console.log("save process validation....");
    var dontSave = false;
    var hasAllegations = false;

    if ($("#pMem1").val()) {
        dontSave = true;
    }
    if ($("#pMem2").val()) {
        dontSave = true;
    }
    if ($("#pMem3").val()) {
        dontSave = true;
    }
    if (parseInt($("#alig").val()) > 0) {
        hasAllegations = true;
    }

    if (dontSave == false) {
        alert('Please add atlest one Panel Member and one Allegation to start the process');
        hasAllegations = false;
    } else if (dontSave == true) {
        if (hasAllegations == false) {
            alert('Please add atlest one Panel Member and one Allegation to start the process');
            dontSave = false;
        } else {
            dontSave = true;
        }
    }

    return dontSave;

    
};




function validateStartProcess() {

    console.log("start process validation....");
    var dontStart = false;
    var hasAllegations = false;

    if ($("#pMem1").val()) {
        dontStart = true;
    }
    if ($("#pMem2").val()) {
        dontStart = true;
    }
    if ($("#pMem3").val()) {
        dontStart = true;
    }

    if (parseInt($("#alig").val()) > 0){
        hasAllegations = true;
    } 

    console.log('Start Process For : ' + $("#pMem1").val());
    console.log('Start Process For : ' + $("#pMem2").val());
    console.log('Start Process For : ' + $("#pMem3").val());
    console.log('Number of aligations : ' + parseInt($("#alig").val()));

    console.log('Start Process Validation : ' + dontStart);

    if (dontStart == false ) {
        alert('Please add atlest one Panel Member and one Allegation to start the process');
        hasAllegations = false;
    } else if (dontStart == true) {
        if (hasAllegations == false) {
            alert('Please add atlest one Panel Member and one Allegation to start the process');
            dontStart = false;
        } else {
            dontStart = true;
        }
    }

    return dontStart;
}


function printDiv(divID) {
    //Get the HTML of div
    var divElements = document.getElementById(divID).innerHTML;
    //Get the HTML of whole page
    var oldPage = document.body.innerHTML;

    //Reset the page's HTML with div's HTML only
    document.body.innerHTML =
      "<html><head><title></title></head><body>" +
      divElements + "</body>";

    //Print Page
    window.print();

    //Restore orignal HTML
    document.body.innerHTML = oldPage;


}