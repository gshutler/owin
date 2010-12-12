require 'rubygems'

task :build do
  src = File.join(File.dirname(__FILE__), 'lib', 'sowin', '*.cs')
  out = File.join(File.dirname(__FILE__), 'build', 'SOWIN.dll')
  sh "gmcs -warnaserror -out:#{out} -target:library -recurse:#{src}" 
end
