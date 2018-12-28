#pragma strict

/*function Start () {

}

function Update () {

}
*/
import System.IO;

//var filePath = "/Users/ResetOfDirectoryPath/testWrite.txt";
var filePath="/sys/class/enable3d/enable-3d";//gadmei
//var filePath="F:/unityfile.txt"; 

function Start() {
try{
    WriteFile(filePath);
    }catch(err ){};
}

 

function WriteFile(filepathIncludingFileName : String)

{

    var sw : StreamWriter = new StreamWriter(filepathIncludingFileName);

//    sw.WriteLine("Line to write");

  //  sw.WriteLine("Another Line");
    sw.WriteLine(System.Convert.ToChar(49));

    sw.Flush();

    sw.Close();

}

 

function ReadFile(filepathIncludingFileName : String) {

    var sr = new File.OpenText(filepathIncludingFileName);

 

    var input = "";

    while (true) {

        input = sr.ReadLine();

        if (input == null) { break; }

        Debug.Log("line="+input);

    }

    sr.Close();

}