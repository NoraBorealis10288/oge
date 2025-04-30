/** File for all of the logic that is accessed by multiple pages */

// file upload code
public static class Shared {
    const int MAX_FILESIZE = 512*1024*1024; // 512MiB, should be significantly more than is necessary for the scope of this project
    public static string uploadStatus = "null";
    public static Dictionary<int,List<ReaderEvent>> dict = new Dictionary<int,List<ReaderEvent>>();
    public static List<ReaderEvent> fileList = new List<ReaderEvent>();

    public static async Task FileUploadedAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile? file) {
        if (file != null) {
            try {
                var stream = file.OpenReadStream(MAX_FILESIZE);

                var path = Path.ChangeExtension(Path.GetTempFileName(),Path.GetExtension(file.Name));
                var destinationStream = new FileStream(file.Name,FileMode.Create);
                await stream.CopyToAsync(destinationStream); //wait until file is finished copying to local scope before intepreting its data
                destinationStream.Close();
                ReadFile(file.Name);
                InitializeDictionary(fileList);
                uploadStatus = "file";
            } catch (Exception exception) {
                Console.WriteLine(exception.Message);
            }
        }
    }

    /** Read the file, one line at a time, skipping the header line,
    and assign each line to a readerevent that is added to the 'file' list */
    static void ReadFile(string filename) {
        try {
            var stream = new StreamReader(filename);
            string currLine = stream.ReadLine();
            currLine = stream.ReadLine();
            while (currLine != null) {
                var readerEvent = new ReaderEvent(currLine);
                fileList.Add(readerEvent);
                currLine = stream.ReadLine();
            }
        } catch (Exception exception) {
            Console.WriteLine(exception.Message);
        }
    }

/** Take the list made by the ReadFile function and use it to initialize a dictionary in which the everything is sorted by the userID */
    static void InitializeDictionary(List<ReaderEvent> list) {
        foreach(ReaderEvent reader in list) {
            if (dict.ContainsKey(reader.GetID())) {
                dict[reader.GetID()].Add(reader);
            } else {
                dict.Add(reader.GetID(), new List<ReaderEvent>());
                dict[reader.GetID()].Add(reader);
            }
        }
    }
}

//code specific to creating the individual reader events that will go into the master dictionary
public class ReaderEvent {
    private string column1 { get; set; }
    private string column2 { get; set; }
    private string column3 { get; set; }
    private string column4 { get; set; }
    private string column5 { get; set; }
    private string column6 { get; set; }

    public ReaderEvent(string line) {
        var temparray = new string[6];
        var cells = line.Split(",");
        for (int i=0; i<cells.Length; i++) {
            if (cells[i] != null) {
                temparray[i] = cells[i];
            } else {
                temparray[i] = "";
            }
        }
        column1 = temparray[0];
        column2 = temparray[1];
        column3 = temparray[2];
        column4 = temparray[3];
        column5 = temparray[4];
        column6 = temparray[5];
    }

    public string GetTimestamp() { return column1; }

    public string GetName() { return column2; }

    public string GetDesc() { return column3; }
    public string GetHash() { return column4; }

    public int GetID() { return Int32.Parse(column5 + column6); }
    public override string ToString() {
        return $"{column1}, {column2}, {column3}, {column4}, {column5}, {column6}";
    }
}