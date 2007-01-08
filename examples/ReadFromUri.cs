using System;
using TagLib;
using Gnome.Vfs;

public class ReadFromUri
{
    public static void Main(string [] args)
    {
        if(args.Length == 0) {
            Console.Error.WriteLine("USAGE: mono ReadFromUri.exe PATH [...]");
            return;
        }
      
        Gnome.Vfs.Vfs.Initialize();
        
        TagLib.File.SetFileAbstractionCreator(new TagLib.File.FileAbstractionCreator(
            VfsFileAbstraction.CreateFile));
            
        try {
            foreach (string path in args)
            {
                string uri = path;
            
                try {
                    System.IO.FileInfo file_info = new System.IO.FileInfo(uri);
                    uri = file_info.FullName;
                } catch {
                }
                
                TagLib.File file = TagLib.File.Create(uri);
                
                Console.WriteLine("Title:      " +  file.Tag.Title);
                Console.WriteLine("Artists:    " + (file.Tag.AlbumArtists == null ? "" : System.String.Join ("\n            ", file.Tag.AlbumArtists)));
                Console.WriteLine("Performers: " + (file.Tag.Performers   == null ? "" : System.String.Join ("\n            ", file.Tag.Performers)));
                Console.WriteLine("Composers:  " + (file.Tag.Composers    == null ? "" : System.String.Join ("\n            ", file.Tag.Composers)));
                Console.WriteLine("Album:      " +  file.Tag.Album);
                Console.WriteLine("Comment:    " +  file.Tag.Comment);
                Console.WriteLine("Genres:     " + (file.Tag.Genres       == null ? "" : System.String.Join ("\n            ", file.Tag.Genres)));
                Console.WriteLine("Year:       " +  file.Tag.Year);
                Console.WriteLine("Track:      " +  file.Tag.Track);
                Console.WriteLine("TrackCount: " +  file.Tag.TrackCount);
                Console.WriteLine("Disc:       " +  file.Tag.Disc);
                Console.WriteLine("DiscCount:  " +  file.Tag.DiscCount + "\n");
                
                Console.WriteLine("Length:     " + file.AudioProperties.Duration);
                Console.WriteLine("Bitrate:    " + file.AudioProperties.Bitrate);
                Console.WriteLine("SampleRate: " + file.AudioProperties.SampleRate);
                Console.WriteLine("Channels:   " + file.AudioProperties.Channels + "\n");
                
                IPicture [] pictures = file.Tag.Pictures;
                
                Console.WriteLine("Embedded Pictures: " + pictures.Length);
                
                foreach(IPicture picture in pictures) {
                    Console.WriteLine(picture.Description);
                    Console.WriteLine("   MimeType: " + picture.MimeType);
                    Console.WriteLine("   Size:     " + picture.Data.Count);
                    Console.WriteLine("   Type:     " + picture.Type);
                }
                
            }
        } finally {
            Gnome.Vfs.Vfs.Shutdown();
        }
    }
}

public class VfsFileAbstraction : TagLib.File.IFileAbstraction
{
    private string name;
    private FilePermissions permissions;

    public VfsFileAbstraction(string file)
    {
        name = file;
        permissions = (new FileInfo(name, FileInfoOptions.FollowLinks | 
            FileInfoOptions.GetAccessRights)).Permissions;

        if(!IsReadable) {
            throw new System.IO.IOException("File \"" + name + "\" is not readable.");
        }
    }

    public string Name {
        get { return name; }
    }

    public System.IO.Stream ReadStream {
        get { return new VfsStream(Name, System.IO.FileMode.Open); }
    }

    public System.IO.Stream WriteStream {
        get { return new VfsStream(Name, System.IO.FileMode.Open); }
    }

    public bool IsReadable {
        get { return (permissions | FilePermissions.AccessReadable) != 0; }
    }

    public bool IsWritable {
        get { return (permissions | FilePermissions.AccessWritable) != 0; }
    }

    public static TagLib.File.IFileAbstraction CreateFile(string path)
    {
        return new VfsFileAbstraction(path);
    }
}
