﻿namespace InformationRadarCore.Models
{
    public class SearchEngine
    {
        Lighthouse lighthouse;

        public SearchEngine ()
        {
            this.lighthouse = new Lighthouse();
        }

        public async Task RunSearch( Lighthouse lighthouse )
        {
            this.lighthouse = lighthouse;

            String[] query = this.lighthouse.GoogleQueries;
            string result;

            String scriptLocation = this.lighthouse.LighthouseScript;

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = String.Format ( "{0}", scriptLocation);
            start.Arguments = string.Format ( "\"{0}\" \"{1}\"", cmd, args );
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            using ( Process process = Process.Start ( start ) )
            {
                using ( StreamReader reader = process.StandardOutput )
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    if ( stderr == null )
                    {
                        result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                        process.WaitForExit ();
                        this.lighthouse.Sites = result;
                    }
                    else
                    {
                        this.lighthouse.Sites = result;
                        this.lighthouse.WriteError = stderr;
                    }
                }
            }
        }

    }
}
