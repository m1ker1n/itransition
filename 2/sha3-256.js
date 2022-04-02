var sha3_256 = require('js-sha3').sha3_256;
var fs = require('fs');

var result = [];

var temp = fs.readdirSync('data','utf-8');
temp.forEach( (file) => {
    result.push(sha3_256(fs.readFileSync('data/'+file)));
});

result.sort();

var str = '';
result.forEach( (hash) => {
    str += hash;
});

var email = 'm1ker1nigor@yandex.ru';
str += email;
str = sha3_256(str);
console.log(str);

/*
> var fs = require('fs'); var sha3_256=require('js-sha3').sha3_256; console.log(sha3_256(fs.readFileSync('data/file_00.data','binary')));
79baf42517b5518db4f36a896b98acf68949fed55f1b5533f1018b48d966f674
*/