var strArr = process.argv.slice(2);
console.log(myFunc(strArr));

function myFunc(arr)
{
    if (!arr.length) 
    {
        return "";
    }

    var word = arr[0];
    var answer = "";
    var wordLength = arr[0].length;

    for(let index = 0; index < wordLength; index++)
    {
        var lastIndex = index;
        var substr = "";

        while (lastIndex <= wordLength)
        {
            substr = word.slice(index,lastIndex);
            if (arr.every(value => value.includes(substr)))
            {
                answer = (answer.length < substr.length)?substr:answer;
            }
            else break;
            lastIndex++;
        }
    }

    return answer;
}