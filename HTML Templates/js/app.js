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
$(document).foundation().foundation('joyride', 'start');

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
$('body').on(eventtype, '.add', function(){
  $('#addNew').toggleClass('hide');
});

//comments
$('body').on(eventtype, '.additional', function(){
  var getID = $(this).attr('data-show');
  $('#'+getID).toggleClass('hide');
});

//expand
$('body').on(eventtype, '.expand', function(){
    var getID = $(this).attr('data-show');
    $('#'+getID).toggleClass('hide');
    $(this).children('i').toggleClass('fa-caret-square-o-up');
});


/* Get current user and set view
--------------------------------------*/
//set initial user as Panel Member
var currentUser = 'Panel Member',
    EditMode = false;

$('.edit').addClass('hide');


//changeview 
function changeView(){
  if (currentUser == 'Case Worker'){
    $('.edit').toggleClass('hide');
    $('.view').toggleClass('hide');    
  } else if (currentUser =='Panel Member'){
    $('.edit').toggleClass('hide');
    $('.view').toggleClass('hide');
  }
}

//checkscreen
function checkScreen(){
  console.log('check')
  if (CommentsScreen == true && EditMode == true){
    console.log('Edit mode on and it is week: '+CurrentWeek);
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').removeClass('hide');
    $('.demo-stage-3').removeClass('hide');
    $('.week-1, .week-4, .week-5, .week-6, .week-6tmp, .week-7 .week-8, .week-9, .week-12').addClass('hide');
    $('.demo-stage-1 .due-countdown').removeClass('hide');  
    $('.demo-stage-1 .due').removeClass('success'); 
    $('.demo-stage-1 .due').html('25/05/2014');
    $('.demo-stage-2 .due').removeClass('success'); 
    $('.demo-stage-2 .due').html('01/06/2014');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('22/06/2014');
  } else if(CommentsScreen == true && EditMode == false) {
    console.log('View mode on and it is week: '+CurrentWeek); 
    switch(CurrentWeek) {
      case '1':
        week1();
        break;
      case '4':
        week4();
        break;
      case '5':
        week5();
        break;
      case '6':
        week6();
        break;
      case '7':
        week7();
        break;
      case '8':
        week8();
        break;
      case '9':
        week9();
        break;
      case '12':
        week12();
        break;
      default:
        console.log('no matches!');
    }
  }else {
    console.log('nothing to see here');
  }
}

//toggle user role when you click the username
$('body').on(eventtype, '#current-user', function(){
  if (currentUser == 'Case Worker'){
    EditMode = false;
    currentUser = 'Panel Member';
    $('#current-user').html(currentUser);
  } else if (currentUser =='Panel Member'){
    EditMode = true;
    currentUser = 'Case Worker';
    $('#current-user').html(currentUser);
  }
  changeView();
  checkScreen();
});



//Simulate Case Progress
function simulateCaseProgress(){
  console.log('View mode on and it is week: '+CurrentWeek);
  $('.demo-stage-2').addClass('hide');
  $('.demo-stage-3').addClass('hide');

  $('body').on('click','.week-1', function(){
    console.log('week 1');
    CurrentWeek = '1';
    week1();
  });
  $('body').on('click','.week-4', function(){
    console.log('week 4');
    CurrentWeek = '4';
    week4();
  });
  $('body').on('click','.week-5', function(){
    console.log('week 5');
    CurrentWeek = '5';
    week5();
  });
  $('body').on('click','.week-6', function(){
    console.log('week 6');
    CurrentWeek = '6';
    week6();
  });
  $('body').on('click','.week-7', function(){
    console.log('week 7');
    CurrentWeek = '7';
    week7();
  });
  $('body').on('click','.week-8', function(){
    console.log('week 8');
    CurrentWeek = '8';
    week8();
  });
  $('body').on('click','.week-9', function(){
    console.log('week 9');
    CurrentWeek = '9';
    week9();
  });
  $('body').on('click','.week-12', function(){
    console.log('week 12');
    CurrentWeek = '12';
    week12();
  });
}

  function week1(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').addClass('hide');
    $('.demo-stage-3').addClass('hide');
    $('.week-4, .week-5, .week-6, .week-6tmp, .week-7 .week-8, .week-9, .week-12').addClass('hide');
    $('.week-1').removeClass('hide');
    $('.demo-stage-1 .due-countdown').removeClass('hide');  
    $('.demo-stage-1 .due').removeClass('success'); 
    $('.demo-stage-1 .due').html('25/05/2014');
    $('.demo-stage-2 .due').removeClass('success'); 
    $('.demo-stage-2 .due').html('Complete');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('22/06/2014');
  };
  function week4(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').addClass('hide');
    $('.demo-stage-3').addClass('hide');
    $('.week-1, .week-5, .week-6, .week-6tmp, .week-7 .week-8, .week-9, .week-12').addClass('hide');
    $('.week-4').removeClass('hide');
    $('.demo-stage-1 .due-countdown').addClass('hide'); 
    $('.demo-stage-1 .due').addClass('success');  
    $('.demo-stage-1 .due').html('Complete');
    $('.demo-stage-2 .due').removeClass('success'); 
    $('.demo-stage-2 .due').html('01/06/2014');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('22/06/2014');
  };
  function week5(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').removeClass('hide');
    $('.demo-stage-3').addClass('hide');
    $('.week-1, .week-6, .week-7, .week-8, .week-9, .week-12').addClass('hide');
    $('.week-4').removeClass('hide');
    $('.week-5').removeClass('hide');     
    $('.week-6tmp').removeClass('hide');
    $('.demo-stage-1 .due-countdown').addClass('hide'); 
    $('.demo-stage-1 .due').addClass('success');  
    $('.demo-stage-1 .due').html('Complete');
    $('.demo-stage-2 .due').removeClass('success'); 
    $('.demo-stage-2 .due').html('01/06/2014');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('22/06/2014');
  };
  function week6(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').removeClass('hide');
    $('.demo-stage-3').addClass('hide');
    $('.week-1, .week-5, .week-7, .week-8, .week-9, .week-12').addClass('hide');
    $('.week-4').removeClass('hide');
    $('.week-6').removeClass('hide');
    $('.week-6tmp').removeClass('hide');
    $('.demo-stage-1 .due-countdown').addClass('hide'); 
    $('.demo-stage-1 .due').addClass('success');  
    $('.demo-stage-1 .due').html('Complete');
    $('.demo-stage-2 .due').addClass('success');  
    $('.demo-stage-2 .due').html('Waiting for Parties Comments');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('22/06/2014');
  };
  function week7(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').removeClass('hide');
    $('.demo-stage-3').addClass('hide');
    $('.week-1, .week-5, .week-6tmp, .week-8, .week-9, .week-12').addClass('hide');
    $('.week-4').removeClass('hide');
    $('.week-6').removeClass('hide');
    $('.week-7').removeClass('hide');
    $('.demo-stage-1 .due-countdown').addClass('hide'); 
    $('.demo-stage-1 .due').addClass('success');  
    $('.demo-stage-1 .due').html('Complete');
    $('.demo-stage-2 .due').addClass('success');  
    $('.demo-stage-2 .due').html('Complete');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('22/06/2014');
  };
  function week8(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').removeClass('hide');
    $('.demo-stage-3').removeClass('hide');
    $('.week-1, .week-5, .week-6tmp, .week-9, .week-12').addClass('hide');
    $('.week-4').removeClass('hide');
    $('.week-6').removeClass('hide');
    $('.week-7').removeClass('hide');
    $('.week-8').removeClass('hide');
    $('.demo-stage-1 .due-countdown').addClass('hide'); 
    $('.demo-stage-1 .due').addClass('success');  
    $('.demo-stage-1 .due').html('Complete');
    $('.demo-stage-2 .due').addClass('success');  
    $('.demo-stage-2 .due').html('Complete');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('22/06/2014');
  };
  function week9(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').removeClass('hide');
    $('.demo-stage-3').removeClass('hide');
    $('.week-1, .week-5, .week-6tmp, .week-8, .week-9, .week-12').addClass('hide');
    $('.week-4').removeClass('hide');
    $('.week-6').removeClass('hide');
    $('.week-7').removeClass('hide');
    $('.demo-stage-1 .due-countdown').addClass('hide'); 
    $('.demo-stage-1 .due').addClass('success');  
    $('.demo-stage-1 .due').html('Complete');
    $('.demo-stage-2 .due').addClass('success');  
    $('.demo-stage-2 .due').html('Complete');
    $('.demo-stage-3 .due').removeClass('success'); 
    $('.demo-stage-3 .due').html('Locked');
  };
  function week12(){
    $('.demo-stage-1').removeClass('hide');
    $('.demo-stage-2').removeClass('hide');
    $('.demo-stage-3').removeClass('hide');
    $('.week-1, .week-5, .week-6tmp, .week-8, .week-9, .week-12').addClass('hide');
    $('.week-4').removeClass('hide');
    $('.week-6').removeClass('hide');
    $('.week-7').removeClass('hide');
    $('.demo-stage-1 .due-countdown').addClass('hide'); 
    $('.demo-stage-1 .due').addClass('success');  
    $('.demo-stage-1 .due').html('Complete');
    $('.demo-stage-2 .due').addClass('success');  
    $('.demo-stage-2 .due').html('Complete');
    $('.demo-stage-3 .due').addClass('success');  
    $('.demo-stage-3 .due').html('Closed');
  };